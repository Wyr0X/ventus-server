using System.Net.WebSockets;
using System.Net;
using System.Collections.Concurrent;
using ProtosCommon;

namespace VentusServer
{
    public class WebSocketServerController
    {
        private readonly FirebaseService _firebaseService;
        private readonly MessageHandler _messageHandler;
        private readonly ConcurrentDictionary<string, WebSocket> _connectedUsers;

        public WebSocketServerController(FirebaseService firebaseService, MessageHandler messageHandler, ConcurrentDictionary<string, WebSocket> connectedUsers)
        {
            _firebaseService = firebaseService;
            _messageHandler = messageHandler;
            _connectedUsers = connectedUsers;
        }

        public async Task StartServerAsync()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:6000/");
            listener.Start();
            Console.WriteLine("✅ Servidor WebSocket iniciado en ws://localhost:6000");

            while (true)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    WebSocket webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                    Console.WriteLine("🔗 Cliente conectado");

                    _ = Task.Run(() => HandleWebSocket(webSocket));
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleWebSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024];
            string? userId = null;

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    using (var ms = new MemoryStream())
                    {
                        do
                        {
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            ms.Write(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            var receivedBytes = ms.ToArray();

                            // Aquí verificamos el token antes de procesar el mensaje
                            var clientMessage = ClientMessage.Parser.ParseFrom(receivedBytes);
                            if (clientMessage.MessageTypeCase == ClientMessage.MessageTypeOneofCase.MessageAuth)
                            {
                                var clientMessageAuth = clientMessage.MessageAuth;

                                var token = clientMessageAuth.Token;
                                userId = await ValidateTokenAndGetUserId(token, webSocket);
                                if (string.IsNullOrEmpty(userId))
                                {
                                    // Token inválido, cerrar conexión
                                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Invalid JWT", CancellationToken.None);
                                    return;
                                }

                            }
                            // Llamamos al MessageHandler para procesar el mensaje solo si el token es válido
                            _messageHandler.HandleIncomingMessage(receivedBytes, webSocket, userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en WebSocket: {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    _connectedUsers.TryRemove(userId, out _);
                    Console.WriteLine($"🔌 Usuario {userId} desconectado.");
                }

                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexión cerrada", CancellationToken.None);
                webSocket.Dispose();
            }
        }

        private async Task<string?> ValidateTokenAndGetUserId(string token, WebSocket webSocket)
        {
            try
            {
                // Verificar el token JWT con Firebase
                var decodedToken = await _firebaseService.VerifyTokenAsync(token);
                var userId = decodedToken.Claims["user_id"]?.ToString();

                if (string.IsNullOrEmpty(userId))
                {
                    return null; // Token inválido o no contiene el campo esperado
                }

                Console.WriteLine($"🔒 Usuario autenticado: {userId}");
                return userId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar JWT: {ex.Message}");
                return null; // Si el token no es válido, se desconecta
            }
        }
    }
}

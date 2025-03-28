using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using FirebaseAdmin.Auth;
using Google.Protobuf;
using ProtosCommon;
using VentusServer;

public class WebSocketServerController
{
    private readonly FirebaseService _firebaseService;
    private readonly ConcurrentDictionary<string, WebSocket> _pendingConnections; // Conexiones pendientes de autenticación
    private readonly ConcurrentDictionary<string, WebSocket> _authenticatedUsers; // Conexiones ya autenticadas
    private readonly ConcurrentQueue<UserMessagePair> _messageQueue; // Conexiones ya autenticadas
    private readonly MessageHandler _messageHandler; // Conexiones ya autenticadas

    public WebSocketServerController(FirebaseService firebaseService,
       MessageHandler messageHandler)
    {
        _firebaseService = firebaseService;
        _pendingConnections = new ConcurrentDictionary<string, WebSocket>();
        _authenticatedUsers = new ConcurrentDictionary<string, WebSocket>();
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _messageHandler = messageHandler;
    }

    public async Task StartServerAsync()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5001/");
        listener.Start();
        Console.WriteLine("✅ Servidor WebSocket iniciado en ws://localhost:6000");

        while (true)
        {

            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                WebSocket webSocket = (await context.AcceptWebSocketAsync(null)).WebSocket;
                // Generar un identificador único para la conexión
                var connectionId = Guid.NewGuid().ToString();
                Console.WriteLine($"🆔 Conexión iniciada con ID único: {connectionId}");

                // Guardar la conexión pendiente en el diccionario
                _pendingConnections[connectionId] = webSocket;

                // Manejar la nueva conexión en un hilo separado y pasar el connectionId
                _ = Task.Run(() => HandleWebSocket(webSocket, connectionId));
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }
    public async Task StartLoop()
    {

        while (true)
        {
            ProcessQueue();
            await Task.Delay(100); // Espera de 100 ms (equivalente a 10 FPS, ajusta según sea necesario)
        }
    }

    private void ProcessQueue()
    {
        // Intentamos sacar un mensaje de la cola

        if (_messageQueue.TryDequeue(out var userMessagePair))
        {
            var clientMessage = ClientMessage.Parser.ParseFrom(userMessagePair.ReceivedData);
            Console.WriteLine($"🔄 Procesando mensajes en la cola. Cantidad actual: {_messageQueue.Count}");

            if (clientMessage.MessageTypeCase == ClientMessage.MessageTypeOneofCase.MessagePing)
            {
                SendServerMessagePong(userMessagePair.ConnectionId, "Pong");
            }
            else
            {
                _messageHandler.HandleIncomingMessage(userMessagePair.ReceivedData, userMessagePair.ConnectionId);

            }
        }
    }
    public async void SendServerPacketBySocket(string userId, IMessage message)
    {
        if (_authenticatedUsers.ContainsKey(userId))
        {
            WebSocket clientSocket = _authenticatedUsers[userId];

            // Serializar el mensaje a un arreglo de bytes
            byte[] serializedMessage = message.ToByteArray();

            // Enviar el mensaje serializado a través del WebSocket
            await clientSocket.SendAsync(new ArraySegment<byte>(serializedMessage), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }

    private async Task HandleWebSocket(WebSocket webSocket, string connectionId)
    {
        var buffer = new byte[1024];
        string? userId = null;

        try
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

                    // Verificar si el paquete es un paquete de autenticación
                    var clientMessage = ClientMessage.Parser.ParseFrom(receivedBytes);
                    Console.WriteLine($"Llego un paquete {clientMessage.MessageTypeCase}");

                    if (clientMessage.MessageTypeCase == ClientMessage.MessageTypeOneofCase.MessageAuth)
                    {
                        var clientMessageAuth = clientMessage.MessageAuth;
                        var clientMessageAuthRequest = clientMessageAuth.AuthRequest;
                        var token = clientMessageAuthRequest.Token;


                        try
                        {
                            // Verificar el token de Firebase
                            FirebaseToken decodedToken = await _firebaseService.VerifyTokenAsync(token);

                            // Extraer el userId (Uid) del token verificado
                            userId = decodedToken.Uid;

                            Console.WriteLine($"🔒 Usuario autenticado: {userId}");

                            if (clientMessageAuth.MessageTypeCase == ClientMessageAuth.MessageTypeOneofCase.AuthRequest)
                            {
                                if (string.IsNullOrEmpty(userId))
                                {
                                    // Si no es un token válido, cerramos la conexión

                                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Token de Firebase inválido", CancellationToken.None);
                                    return;
                                }
                                if (_pendingConnections.TryRemove(connectionId, out var pendingWebSocket))
                                {
                                    _authenticatedUsers[connectionId] = pendingWebSocket;
                                    Console.WriteLine($"🔗 Usuario {userId} añadido a la lista de usuarios autenticados.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine($"❌ Error al verificar el token: {ex.Message}");
                            // Si la verificación falla, cerramos la conexión
                            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Token de Firebase inválido", CancellationToken.None);
                            return;
                        }
                    }

                }
            }

            // Aquí podemos manejar otros mensajes del usuario una vez autenticado
            while (webSocket.State == WebSocketState.Open)
            {
                var ms = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                var receivedData = ms.ToArray();

                // Verificar si la conexión está autenticada
                if (_authenticatedUsers.ContainsKey(connectionId))
                {
                    // Procesar mensaje del usuario autenticado
                    UserMessagePair messagePair = new UserMessagePair(connectionId, receivedData);
                    Console.WriteLine("Enqueue");
                    _messageQueue.Enqueue(messagePair);

                }
                else
                {

                    // Si la conexión no está autenticada, cerrar la conexión
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Usuario no autenticado", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en WebSocket: {ex.Message}");
        }
        finally
        {
            // Si el usuario estaba autenticado, lo eliminamos de la lista
            if (!string.IsNullOrEmpty(userId))
            {
                _authenticatedUsers.TryRemove(userId, out _);
                Console.WriteLine($"🔌 Usuario {userId} desconectado.");
            }
            // Si estaba pendiente, lo eliminamos también
            _pendingConnections.TryRemove(connectionId, out _);

            // Cerramos el WebSocket
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexión cerrada", CancellationToken.None);
            webSocket.Dispose();
        }
    }

    private void SendServerMessagePong( string userId, string message)
    {
        ServerMessagePong pongMessage = new ServerMessagePong
        {
            Message = message
        };
        ServerMessage serverMessage = new ServerMessage
        {
            ServerMessagePong = pongMessage
        };
        SendServerPacketBySocket(userId, serverMessage);

    }
}
public class UserMessagePair
{
    public string ConnectionId { get; set; }
    public byte[] ReceivedData { get; set; }

    public UserMessagePair(string connectionId, byte[] receivedData)
    {
        ConnectionId = connectionId;
        ReceivedData = receivedData;
    }
}

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
    private readonly ConcurrentDictionary<Guid, WebSocket> _websocketsByAccountId;
    private readonly ConcurrentDictionary<string, Guid> _connectionsByAccountId;
    private readonly ConcurrentQueue<UserMessagePair> _messageQueue; // Conexiones ya autenticadas
    private readonly MessageHandler _messageHandler; // Conexiones ya autenticadas
    private readonly JwtService _jwtService;
    public WebSocketServerController(FirebaseService firebaseService, MessageHandler messageHandler
        , JwtService jwtService)
    {
        _firebaseService = firebaseService;
        _pendingConnections = new ConcurrentDictionary<string, WebSocket>();
        _websocketsByAccountId = new ConcurrentDictionary<Guid, WebSocket>();
        _connectionsByAccountId = new ConcurrentDictionary<string, Guid>();
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _messageHandler = messageHandler;
        _jwtService = jwtService;
    }

    public async Task StartServerAsync()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5331/");
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
    public async void ProcessQueue()
    {
        // Intentamos sacar un mensaje de la cola
        if (_messageQueue.TryDequeue(out var userMessagePair))
        {
            _messageHandler.HandleIncomingMessage(userMessagePair);
        }
    }

    public async void SendServerPacketByAccountId(Guid accountId, IMessage message)
    {
        if (_websocketsByAccountId.ContainsKey(accountId))
        {
            WebSocket clientSocket = _websocketsByAccountId[accountId];

            // Serializar el mensaje a un arreglo de bytes
            byte[] serializedMessage = message.ToByteArray();
            // Enviar el mensaje serializado a través del WebSocket
            await clientSocket.SendAsync(
                new ArraySegment<byte>(serializedMessage),
                WebSocketMessageType.Binary,
                true,
                CancellationToken.None
            );
        }
    }

    private async Task HandleWebSocket(WebSocket webSocket, string connectionId)
    {
        var buffer = new byte[1024];
        Guid accountId = Guid.Empty; 

        try
        {
            WebSocketReceiveResult result;
            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None
                    );
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    var receivedBytes = ms.ToArray();

                    // Verificar si el paquete es un paquete de autenticación
                    var clientMessage = ClientMessage.Parser.ParseFrom(receivedBytes);
                    Console.WriteLine($"Llego un paquete {clientMessage.MessageTypeCase}");

                    if (
                        clientMessage.MessageTypeCase
                        == ClientMessage.MessageTypeOneofCase.MessageAuth
                    )
                    {
                        var clientMessageAuth = clientMessage.MessageAuth;
                        var clientMessageAuthRequest = clientMessageAuth.AuthRequest;
                        var token = clientMessageAuthRequest.Token;

                        try
                        {

                            var validatedAccountIdStr = _jwtService.ValidateToken(token);
                            if (!Guid.TryParse(validatedAccountIdStr, out accountId))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("⚠ Token inválido.");
                                Console.ResetColor();
                               
                                return;
                            }

                            // Extraer el userId (Uid) del token verificado

                            Console.WriteLine($"🔒 Usuario autenticado: {accountId}");

                            if (
                                clientMessageAuth.MessageTypeCase
                                == ClientMessageAuth.MessageTypeOneofCase.AuthRequest
                            )
                            {
                               if (accountId == Guid.Empty)
                                {
                                    // Si no es un token válido, cerramos la conexión

                                    await webSocket.CloseAsync(
                                        WebSocketCloseStatus.InvalidMessageType,
                                        "Token de Firebase inválido",
                                        CancellationToken.None
                                    );
                                    return;
                                }
                                if (
                                    _pendingConnections.TryRemove(
                                        connectionId,
                                        out var pendingWebSocket
                                    )
                                )
                                {
                                    _connectionsByAccountId[connectionId] = accountId;
                                    _websocketsByAccountId[accountId] = pendingWebSocket;
                                    Console.WriteLine(
                                        $"🔗 Usuario {accountId} añadido a la lista de usuarios autenticados."
                                    );
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Error al verificar el token: {ex.Message}");
                            // Si la verificación falla, cerramos la conexión
                            await webSocket.CloseAsync(
                                WebSocketCloseStatus.InvalidMessageType,
                                "Token de Firebase inválido",
                                CancellationToken.None
                            );
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
                    result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None
                    );
                    ms.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                var receivedData = ms.ToArray();

                // Verificar si la conexión está autenticada
                if (_connectionsByAccountId.ContainsKey(connectionId) && (accountId != Guid.Empty))
                {
                    // Procesar mensaje del usuario autenticado
                    var clientMessage = ClientMessage.Parser.ParseFrom(receivedData);

                    UserMessagePair messagePair = new UserMessagePair(accountId, clientMessage);
                    Console.WriteLine("Enqueue");
                    _messageQueue.Enqueue(messagePair);
                }
                else
                {
                    // Si la conexión no está autenticada, cerrar la conexión
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.InvalidMessageType,
                        "Usuario no autenticado",
                        CancellationToken.None
                    );
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
            if (accountId != Guid.Empty)
            {
                _connectionsByAccountId.TryRemove(connectionId, out _);
                _websocketsByAccountId.TryRemove(accountId, out _);

                Console.WriteLine($"🔌 Usuario {accountId} desconectado.");
            }
            // Si estaba pendiente, lo eliminamos también
            _pendingConnections.TryRemove(connectionId, out _);

            // Cerramos el WebSocket
            await webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Conexión cerrada",
                CancellationToken.None
            );
            webSocket.Dispose();
        }
    }
}


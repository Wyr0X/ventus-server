using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Common;

public class WebSocketServerController
{
    private readonly WebSocketAuthenticationService _authService;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ConcurrentQueue<UserMessagePair> _messageQueue;
    private readonly MessageHandler _messageHandler;

    public WebSocketServerController(MessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _authService = new WebSocketAuthenticationService();
        _connectionManager = new WebSocketConnectionManager();
    }

    public async Task StartServerAsync()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5331/");
        listener.Start();

        LoggerUtil.Log("WebSocket", "✅ WebSocket iniciado en ws://localhost:5331", ConsoleColor.Green);

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var connectionId = Guid.NewGuid().ToString();
                var socket = (await context.AcceptWebSocketAsync(null)).WebSocket;

                LoggerUtil.Log("WebSocket", $"🆔 Conexión iniciada: {connectionId}", ConsoleColor.Yellow);

                _connectionManager.AddPendingConnection(connectionId, socket);
                _ = Task.Run(() => HandleWebSocket(socket, connectionId));
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
                LoggerUtil.Log("WebSocket", "❌ Petición no válida (400)", ConsoleColor.Red);
            }
        }
    }

    public async Task StartLoop()
    {
        while (true)
        {
            if (_messageQueue.TryDequeue(out var pair))
                _messageHandler.HandleIncomingMessage(pair);
            await Task.Delay(100);
        }
    }

    public async void SendServerPacketByAccountId(Guid accountId, IMessage message)
    {
        if (_connectionManager.TryGetSocket(accountId, out var socket))
        {
            byte[] data = message.ToByteArray();
            await socket.SendAsync(new(data), WebSocketMessageType.Binary, true, CancellationToken.None);
            LoggerUtil.Log("WebSocket", $"📤 Enviado mensaje a {accountId}: {message.GetType().Name}", ConsoleColor.Blue);
        }
        else
        {
            LoggerUtil.Log("WebSocket", $"❌ No se encontró WebSocket para {accountId}", ConsoleColor.Red);
        }
    }

    public void SendBroadcast(List<Guid> accountIds, IMessage message)
    {
        foreach (var id in accountIds)
        {
            SendServerPacketByAccountId(id, message);
        }
    }

    private async Task HandleWebSocket(WebSocket socket, string connectionId)
    {
        var buffer = new byte[1024];
        Guid accountId = Guid.Empty;

        try
        {
            using var ms = new MemoryStream();
            var result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
            ms.Write(buffer, 0, result.Count);
            if (result.MessageType != WebSocketMessageType.Binary)
            {
                LoggerUtil.Log("WebSocket", $"❌ Mensaje no binario recibido de {connectionId}", ConsoleColor.Red);
                return;
            }

            var clientMessage = ClientMessage.Parser.ParseFrom(ms.ToArray());

            if (clientMessage.MessageCase == ClientMessage.MessageOneofCase.AuthRequest)
            {
                if (!_authService.TryAuthenticate(clientMessage.AuthRequest.Token, out accountId))
                {
                    LoggerUtil.Log("Auth", $"❌ Token inválido recibido de {connectionId}", ConsoleColor.Red);
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Token inválido", CancellationToken.None);
                    return;
                }

                if (!_connectionManager.TryAuthenticateConnection(connectionId, accountId, out socket))
                {
                    LoggerUtil.Log("Auth", $"❌ Fallo al autenticar conexión {connectionId}", ConsoleColor.Red);
                    await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "No se pudo autenticar", CancellationToken.None);
                    return;
                }

                LoggerUtil.Log("Auth", $"🔓 Usuario autenticado: {accountId}", ConsoleColor.Green);
            }

            while (socket.State == WebSocketState.Open)
            {
                ms.SetLength(0);
                result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
                ms.Write(buffer, 0, result.Count);

                if (!_connectionManager.TryGetAccountId(connectionId, out accountId))
                {
                    LoggerUtil.Log("WebSocket", $"❌ Usuario no autenticado para {connectionId}", ConsoleColor.Red);
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Usuario no autenticado", CancellationToken.None);
                    return;
                }

                clientMessage = ClientMessage.Parser.ParseFrom(ms.ToArray());

                if (clientMessage.MessageCase == ClientMessage.MessageOneofCase.ClientMessagePing)
                {
                    var pong = new ServerMessage { ServerMessagePong = new ServerMessagePong { Message = "Pong" } };
                    SendServerPacketByAccountId(accountId, pong);
                    LoggerUtil.Log("[Message]", "Ping Recibido", ConsoleColor.Magenta);
                }
                else
                {
                    LoggerUtil.Log("WebSocket", $"📥 Mensaje recibido de {accountId}: {clientMessage.MessageCase}", ConsoleColor.Cyan);
                    _messageQueue.Enqueue(new UserMessagePair(accountId, clientMessage));
                }
            }
        }
        catch (Exception ex)
        {
            LoggerUtil.Log("WebSocket", $"❌ Error en WebSocket {connectionId}: {ex.Message}", ConsoleColor.Red);
        }
        finally
        {
            if (accountId != Guid.Empty)
            {
                _connectionManager.RemoveConnection(connectionId, accountId);
                LoggerUtil.Log("WebSocket", $"🔌 Conexión cerrada para {accountId}", ConsoleColor.DarkYellow);
            }
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrado", CancellationToken.None);
            socket.Dispose();
        }
    }
}

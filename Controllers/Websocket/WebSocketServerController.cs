using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using Google.Protobuf;
using Grpc.Core;
using Protos.Auth;
using Ventus.Client;
using Ventus.Server;
using Ventus.Server.Common;
using VentusServer.Services;

public class WebSocketServerController
{
    private readonly WebSocketAuthenticationService _authService;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ConcurrentQueue<UserMessagePair> _messageQueue;
    private readonly MessageDispatcher _messageDispatcher;

    public WebSocketServerController(MessageDispatcher messageDispatcher, AccountService accountService)
    {
        _messageDispatcher = messageDispatcher;
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _authService = new WebSocketAuthenticationService(accountService);
        _connectionManager = new WebSocketConnectionManager();
    }

    public async Task StartServerAsync()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5331/");
        listener.Start();

        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, "✅ WebSocket iniciado en ws://localhost:5331");

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var connectionId = Guid.NewGuid().ToString();
                var socket = (await context.AcceptWebSocketAsync(null)).WebSocket;

                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🆔 Conexión iniciada: {connectionId}");

                _connectionManager.AddPendingConnection(connectionId, socket);
                _ = Task.Run(() => HandleWebSocket(socket, connectionId));
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, "❌ Petición no válida (400)");
            }
        }
    }

    public async Task StartLoop()
    {
        while (true)
        {
            if (_messageQueue.TryDequeue(out var pair))
                _messageDispatcher.Dispatch(pair);
            await Task.Delay(100);
        }
    }

    public async void SendServerPacketByAccountId(Guid accountId, ServerMessage message)
    {


        if (_connectionManager.TryGetSocket(accountId, out var socket))
        {

            byte[] data = message.ToByteArray();
            await socket.SendAsync(new(data), WebSocketMessageType.Binary, true, CancellationToken.None);
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"📤 Enviado mensaje a {accountId}: {message.PayloadCase}");
        }
        else
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ No se encontró WebSocket para {accountId}");
        }

    }

    public void SendBroadcast(List<Guid> accountIds, ServerMessage message)
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
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Mensaje no binario recibido de {connectionId}");
                return;
            }

            var clientMessage = ClientMessage.Parser.ParseFrom(ms.ToArray());
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Bytes :{ms.ToArray()}");

            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Paquete recibido - Tipo :{clientMessage.PayloadCase}");
            if (clientMessage.PayloadCase == ClientMessage.PayloadOneofCase.LoginRequest)
            {
                if (!_authService.TryAuthenticate(clientMessage.LoginRequest.Token, out accountId))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Token inválido recibido de {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Token inválido", CancellationToken.None);
                    return;
                }
                await _connectionManager.RemoveConnectionByAccountId(accountId);

                if (!_connectionManager.TryAuthenticateConnection(connectionId, accountId, clientMessage.LoginRequest.Token, out socket))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Fallo al autenticar conexión {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "No se pudo autenticar", CancellationToken.None);
                    return;
                }
                SendWebSocketStatusOpen(accountId);
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🔓 Usuario autenticado: {accountId}");
            }

            while (socket.State == WebSocketState.Open)
            {
                ms.SetLength(0);
                result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
                ms.Write(buffer, 0, result.Count);
                clientMessage = ClientMessage.Parser.ParseFrom(ms.ToArray());

                if (!_connectionManager.TryGetAccountId(connectionId, out accountId))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Usuario no autenticado para {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Usuario no autenticado", CancellationToken.None);
                    return;
                }
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Paquete recibido - Tipo :{clientMessage.PayloadCase}");

                if (clientMessage.PayloadCase == ClientMessage.PayloadOneofCase.MessagePing)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"📥 Ping recibido de {accountId}");
                    var pong = new ServerMessage { MessagePong = new MessagePong { Message = "Pong" } };
                    await socket.SendAsync(new(pong.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                }

                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"📥 Mensaje recibido de {accountId}: {clientMessage.PayloadCase}");
                    _messageQueue.Enqueue(new UserMessagePair(accountId, clientMessage));
                }
            }
        }
        catch (Exception ex)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Error en WebSocket {connectionId}: {ex.Message}");
        }
        finally
        {
            if (accountId != Guid.Empty)
            {
                _connectionManager.RemoveConnection(connectionId, accountId);
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🔌 Conexión cerrada para {accountId}");
            }
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cerrado", CancellationToken.None);
            socket.Dispose();
        }
    }
    public void SendWebSocketStatusOpen(Guid accountId)
    {
        LoginResponse loginResponse = new LoginResponse
        {
            Success = true
        };
        ServerMessage serverMessage = new ServerMessage { LoginResponse = loginResponse };
        SendServerPacketByAccountId(accountId, serverMessage);
    }
    public void SendSessionInvalidate(Guid accountId)
    {
        StatusMessage serverStatusMessage = new StatusMessage
        {
            Level = NotificationLevel.Info.GetDescription(),
            Message = "Tu cuenta ha iniciado sesión desde otro lugar. Has sido desconectado.",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        ServerMessage serverMessage = new ServerMessage { StatusMessage = serverStatusMessage };
        SendServerPacketByAccountId(accountId, serverMessage);

    }
    public async Task RemoveConnectionByAccountId(Guid accountId)
    {
        await _connectionManager.RemoveConnectionByAccountId(accountId);
        SendSessionInvalidate(accountId);
    }
}

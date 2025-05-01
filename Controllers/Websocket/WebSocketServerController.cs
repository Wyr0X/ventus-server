using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using Google.Protobuf;
using Grpc.Core;
using Ventus.Network.Packets;
using VentusServer.Services;

public class WebSocketServerController
{
    private readonly WebSocketAuthenticationService _authService;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ConcurrentQueue<UserMessagePair> _messageQueue;
    private readonly TaskScheduler _taskScheduler;

    public WebSocketServerController(TaskScheduler taskScheduler, AccountService accountService)
    {
        _taskScheduler = taskScheduler;
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _authService = new WebSocketAuthenticationService(accountService);
        _connectionManager = new WebSocketConnectionManager();
    }

    public async Task StartServerAsync(CancellationToken cancellationToken)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5331/");
        listener.Start();

        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, "✅ WebSocket iniciado en ws://localhost:5331");

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
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

    public async Task StartLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_messageQueue.TryDequeue(out var pair))
                _taskScheduler.Dispatch(pair);
            await Task.Delay(100);
        }
    }

    public async void SendServerPacketByAccountId(Guid accountId, IMessage message, ServerPacket serverPacket)
    {


        if (_connectionManager.TryGetSocket(accountId, out var socket))
        {

            var serverPackert = new Packet { Type = (uint)serverPacket, Payload = Google.Protobuf.ByteString.CopyFrom(message.ToByteArray()) };
            await socket.SendAsync(new ArraySegment<byte>(serverPackert.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        else
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ No se encontró WebSocket para {accountId}");
        }

    }

    public void SendBroadcast(List<Guid> accountIds, IMessage message, ServerPacket serverPacket)
    {
        foreach (var id in accountIds)
        {
            SendServerPacketByAccountId(id, message, serverPacket);
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

            var packet = Packet.Parser.ParseFrom(ms.ToArray());
            var packetType = (ClientPacket)packet.Type;
            IMessage clientPacket = ClientPacketDecoder.Parsers[packetType].ParseFrom(packet.Payload);

            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Bytes :{ms.ToArray()}");

            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Paquete recibido - Tipo :{packetType}");
            if (packetType == ClientPacket.LoginRequest)
            {
                var loginPacket = (LoginRequest)clientPacket;
                if (!_authService.TryAuthenticate(loginPacket.Token, out accountId))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Token inválido recibido de {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Token inválido", CancellationToken.None);
                    return;
                }
                await _connectionManager.RemoveConnectionByAccountId(accountId);

                if (!_connectionManager.TryAuthenticateConnection(connectionId, accountId, loginPacket.Token, out socket))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Fallo al autenticar conexión {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "No se pudo autenticar", CancellationToken.None);
                    return;
                }
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🔓 Usuario autenticado: {accountId}");
            }

            while (socket.State == WebSocketState.Open)
            {
                ms.SetLength(0);
                result = await socket.ReceiveAsync(new(buffer), CancellationToken.None);
                ms.Write(buffer, 0, result.Count);
                packet = Packet.Parser.ParseFrom(ms.ToArray());
                packetType = (ClientPacket)packet.Type;
                clientPacket = ClientPacketDecoder.Parsers[packetType].ParseFrom(packet.Payload);


                if (!_connectionManager.TryGetAccountId(connectionId, out accountId))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"❌ Usuario no autenticado para {connectionId}");
                    await socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Usuario no autenticado", CancellationToken.None);
                    return;
                }
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"Paquete recibido - Tipo :{packetType}");

                if (packetType == ClientPacket.MessagePing)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"📥 Ping recibido de {accountId}");
                    var pong = new MessagePong { Message = "Pong" };
                    var pongPacket = new Packet { Payload = Google.Protobuf.ByteString.CopyFrom(pong.ToByteArray()), Type = (uint)ServerPacket.MessagePong };
                    await socket.SendAsync(new(pongPacket.ToByteArray()), WebSocketMessageType.Binary, true, CancellationToken.None);
                }

                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"📥 Mensaje recibido de {accountId}: {packetType}");
                    _messageQueue.Enqueue(new UserMessagePair(accountId, clientPacket, packetType));
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

    public async Task RemoveConnectionByAccountId(Guid accountId)
    {
        await _connectionManager.RemoveConnectionByAccountId(accountId);
    }
}

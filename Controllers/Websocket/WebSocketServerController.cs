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
    public readonly IAccountService _accountService;
    public readonly OutgoingMessageQueue _outgoingQueue;

    private static readonly HashSet<ClientPacket> _silentPackets = new HashSet<ClientPacket>
    {
        ClientPacket.PlayerInput
        // añade aquí más packets que quieras silenciar...
    };


    public WebSocketServerController(TaskScheduler taskScheduler, IAccountService IAccountService)
    {
        _taskScheduler = taskScheduler;
        _messageQueue = new ConcurrentQueue<UserMessagePair>();
        _authService = new WebSocketAuthenticationService(IAccountService);
        _connectionManager = new WebSocketConnectionManager();
        _outgoingQueue = new OutgoingMessageQueue(); // Initialize the _outgoingQueue field
        _accountService = IAccountService;
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

    private readonly SemaphoreSlim _sendSemaphore = new(5); // máximo 5 tareas en paralelo
    public void StartLoop(CancellationToken cancellationToken)
    {
        Task.Run(() => StartIncomingLoop(cancellationToken), cancellationToken);
        Task.Run(() => StartOutgoingLoop(cancellationToken), cancellationToken);
    }

    private async Task StartIncomingLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_messageQueue.TryDequeue(out var pair))
            {
                _taskScheduler.Dispatch(pair);
            }

            await Task.Delay(1, cancellationToken); // Micro descanso para evitar CPU 100%
        }
    }
    private async Task StartOutgoingLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _sendSemaphore.WaitAsync(cancellationToken);
            try
            {
                await SendOutgoingPackets(cancellationToken);
            }
            finally
            {
                _sendSemaphore.Release();
            }

            await Task.Delay(30, cancellationToken); // Ritmo constante de envío
        }
    }
    public async Task SendOutgoingPackets(CancellationToken cancellationToken)
    {


        var accountIds = _outgoingQueue.GetAccountIdsWithMessages();
        foreach (var accountId in accountIds)
        {
            if (!_connectionManager.TryGetSocket(accountId, out var socket) || socket.State != WebSocketState.Open)
            {
                continue;
            }
            using var ms = new MemoryStream();


            var packetsToSend = new List<Packet>();

            while (_outgoingQueue.TryDequeue(accountId, out var outgoingPacket))
            {
                if (socket.State != WebSocketState.Open)
                    break; // o continue= ByteString.CopyFrom(outgoingPacket.Message.ToByteArray())

                if (outgoingPacket == null) return;
                var packet = new Packet
                {
                    Type = (uint)outgoingPacket.PacketType,
                    Payload = ByteString.CopyFrom(outgoingPacket.Message.ToByteArray())
                };

                packet.WriteDelimitedTo(ms);
                packetsToSend.Add(packet);

            }


            await socket.SendAsync(
                new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length),
                WebSocketMessageType.Binary,
                true,
                CancellationToken.None
            );
        }

        await Task.Delay(30); // Ajustá esto según la frecuencia de envío deseada
    }

    /*************  ✨ Windsurf Command ⭐  *************/
    /// <summary>
    /// Envía un paquete al cliente conectado con el accountId especificado.
    /// </summary>
    /// <param name="accountId">Id del cuenta del cliente que se le enviará el paquete.</param>
    /// <param name="message">Mensaje que se serializará y enviará en el paquete.</param>

    /*******  58de4cea-744b-4f12-96f0-affe70fcf2e2  *******/
    public async Task SendServerPacketByAccountId(Guid accountId, IMessage message, ServerPacket serverPacket)
    {


        if (_connectionManager.TryGetSocket(accountId, out var socket))
        {

            var serverPackert = new Packet { Type = (uint)serverPacket, Payload = Google.Protobuf.ByteString.CopyFrom(message.ToByteArray()) };
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🆔Packet type to send {(uint)serverPacket}");

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
        // Buffer más generoso y ArraySegment reutilizable
        var buffer = new byte[4096];
        var bufferSegment = new ArraySegment<byte>(buffer);
        Guid accountId = Guid.Empty;

        try
        {
            // 1) Primer paquete: LOGIN
            var loginPkt = await ReceivePacket(socket, bufferSegment);
            if (loginPkt == null || loginPkt.Type != (uint)ClientPacket.LoginRequest)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                    $"❌ Se esperaba LoginRequest, llegó tipo={(loginPkt?.Type.ToString() ?? "null")}");
                await CloseAndCleanupSocketAsync(socket, connectionId, accountId,
                    "Login no recibido", WebSocketCloseStatus.PolicyViolation);
                return;
            }

            // Parsear y autenticar
            var loginParser = ClientPacketDecoder.Parsers[ClientPacket.LoginRequest];
            var loginRequest = (LoginRequest)loginParser.ParseFrom(loginPkt.Payload);
            if (!_authService.TryAuthenticate(loginRequest.Token, out accountId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                    $"❌ Token inválido para {connectionId}");
                await CloseAndCleanupSocketAsync(socket, connectionId, accountId,
                    "Token inválido", WebSocketCloseStatus.NormalClosure);
                return;
            }

            // Registrar conexión autenticada
            await _connectionManager.RemoveConnectionByAccountId(accountId);
            if (!_connectionManager.TryAuthenticateConnection(connectionId, accountId,
                    loginRequest.Token, out socket))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                    $"❌ Falló TryAuthenticateConnection {connectionId}");
                await CloseAndCleanupSocketAsync(socket, connectionId, accountId,
                    "Auth connection fail", WebSocketCloseStatus.NormalClosure);
                return;
            }
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                $"🔓 Usuario autenticado: {accountId}");

            // 2) Bucle principal de recepción y despacho
            while (socket.State == WebSocketState.Open)
            {
                var pkt = await ReceivePacket(socket, bufferSegment);
                if (pkt == null)
                    break; // Close frame o error de lectura
                var packetType = (ClientPacket)pkt.Type;
                var clientPacket = ClientPacketDecoder.Parsers[packetType].ParseFrom(pkt.Payload);
                var typeEnum = (ClientPacket)pkt.Type;
                if (!ClientPacketDecoder.Parsers.TryGetValue(typeEnum, out var parser))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                        $"❌ Paquete desconocido: {pkt.Type}");
                    continue;
                }

                if (!_silentPackets.Contains(typeEnum))
                {
                    LoggerUtil.Log(
                        LoggerUtil.LogTag.WebSocketServerController,
                        $"📥 Paquete recibido – Tipo: {typeEnum}"
                    );
                }
                // Caso Ping
                if (typeEnum == ClientPacket.MessagePing)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                        $"📥 Ping de {accountId}");
                    var pong = new MessagePong { Message = "Pong" };
                    var pongPacket = new Packet
                    {
                        Type = (uint)ServerPacket.MessagePong,
                        Payload = ByteString.CopyFrom(pong.ToByteArray())
                    };
                    _outgoingQueue.Enqueue(accountId, pongPacket, ServerPacket.MessagePong);
                }
                else
                {
                    // Encolar para el dispatcher
                    _messageQueue.Enqueue(new UserMessagePair(accountId, clientPacket, typeEnum));
                }
            }
        }
        catch (Exception ex)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                $"❌ Error en WebSocket {connectionId}: {ex.Message}");
        }
        finally
        {
            // Limpieza final
            if (accountId != Guid.Empty)
            {
                _connectionManager.RemoveConnection(connectionId, accountId);
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController,
                    $"🔌 Conexión cerrada para {accountId}");
            }
            await CloseAndCleanupSocketAsync(socket, connectionId, accountId,
                "Conexión finalizada", WebSocketCloseStatus.NormalClosure);
        }
    }

    // Helper que lee hasta EndOfMessage y filtra control-frames.
    // Devuelve null si llegó un Close o un frame no-binario.
    private async Task<Packet?> ReceivePacket(WebSocket socket, ArraySegment<byte> bufferSegment)
    {
        using var ms = new MemoryStream();
        WebSocketReceiveResult result;

        do
        {
            result = await socket.ReceiveAsync(bufferSegment, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
                return null;

            if (result.MessageType != WebSocketMessageType.Binary)
                return null;  // Ignora Ping/Pong en texto o control

            ms.Write(bufferSegment.Array!, 0, result.Count);
        }
        while (!result.EndOfMessage);

        var raw = ms.ToArray();
        return Packet.Parser.ParseFrom(raw);
    }


    public async Task RemoveConnectionByAccountId(Guid accountId)
    {
        await _connectionManager.RemoveConnectionByAccountId(accountId);
    }
    private async Task CloseAndCleanupSocketAsync(WebSocket socket, string connectionId, Guid accountId, string reason, WebSocketCloseStatus status)
    {
        try
        {
            if (socket?.State == WebSocketState.Open || socket?.State == WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(status, reason, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"⚠️ Error al cerrar socket de {accountId}: {ex.Message}");
        }
        finally
        {
            socket?.Dispose();

            if (accountId != Guid.Empty)
            {
                await _connectionManager.RemoveConnectionByAccountId(accountId);
                UserMessagePair userMessagePair = new(accountId, new TryToDespawnPlayer(), ClientPacket.TryToDespawnPlayer);
                _taskScheduler.Dispatch(userMessagePair);
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🔌 Conexión cerrada para {accountId}");
            }
            else if (!string.IsNullOrEmpty(connectionId))
            {
                _connectionManager.RemoveConnection(connectionId, accountId);
                UserMessagePair userMessagePair = new(accountId, new TryToDespawnPlayer(), ClientPacket.TryToDespawnPlayer);
                _taskScheduler.Dispatch(userMessagePair);
                LoggerUtil.Log(LoggerUtil.LogTag.WebSocketServerController, $"🔌 Conexión pendiente cerrada: {connectionId}");
            }
        }
    }


}

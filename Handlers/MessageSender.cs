using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Auth;
using Protos.Common;
using Protos.Game.Session;
using Protos.Game.World;
using Microsoft.Extensions.Logging; // Necesario para ILogger

public class MessageSender
{
    private readonly WebSocketServerController _websocket;
    private readonly ILogger<MessageSender> _logger;

    public MessageSender(WebSocketServerController webSocket, ILogger<MessageSender> logger)
    {
        _websocket = webSocket;
        _logger = logger;
    }

    public void SendAuthResponse(Guid accountId, bool success)
    {
        _logger.LogInformation("🔐 Enviando AuthResponse a {AccountId}, éxito: {Success}", accountId, success);

        var authResponse = new AuthResponse
        {
            Success = success
        };

        var serverMessage = new ServerMessage
        {
            AuthResponse = authResponse
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendPlayerPosition(Guid accountId, int playerId, float x, float y)
    {
        _logger.LogInformation("📍 Enviando posición del jugador {PlayerId} a {AccountId}: ({X}, {Y})", playerId, accountId, x, y);

        var playerPosition = new PlayerPosition
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };

        var serverMessageGameSession = new ServerMessageGameSession
        {
            PlayerPosition = playerPosition
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessageGameSession);
    }

    public void SpawnPlayer(Guid accountId, int playerId, float x, float y)
    {
        _logger.LogInformation("🧍 Spawn de jugador {PlayerId} para {AccountId} en ({X}, {Y})", playerId, accountId, x, y);

        var playerSpawn = new PlayerSpawn
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };

        var serverMessageGameSession = new ServerMessageGameSession
        {
            PlayerSpawn = playerSpawn
        };

        var serverMessage = new ServerMessage
        {
            ServerMessageSession = serverMessageGameSession
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendWorlState(List<Guid> accountsIds, WorldStateUpdate worldStateUpdate)
    {
        _logger.LogInformation("🌍 Enviando estado del mundo a {Count} usuarios", accountsIds.Count);

        var serverWorldMessage = new ServerWorldMessage
        {
            WorldStateUpdate = worldStateUpdate
        };

        var serverMessage = new ServerMessage
        {
            ServerWorldMessage = serverWorldMessage
        };

        _websocket.SendBroadcast(accountsIds, serverMessage);
    }
}

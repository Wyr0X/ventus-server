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

    public MessageSender(WebSocketServerController webSocket)
    {
        _websocket = webSocket;
    }

    public void SendAuthResponse(Guid accountId, bool success)
    {

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
            Console.WriteLine($"Entra 3");

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendWorlState(List<Guid> accountsIds, WorldStateUpdate worldStateUpdate)
    {

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

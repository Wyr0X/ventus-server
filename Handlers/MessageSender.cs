using Ventus.Server;
using Protos.Game.Server.Session;
using Grpc.Core;
using Protos.Game.Server.Movement;
using Protos.Auth; // Necesario para ILogger

public class MessageSender
{
    private readonly WebSocketServerController _websocket;

    public MessageSender(WebSocketServerController webSocket)
    {
        _websocket = webSocket;
    }

    public void SendAuthResponse(Guid accountId, bool success)
    {

        var loginResponse = new LoginResponse
        {
            Success = success
        };

        var serverMessage = new ServerMessage
        {
            LoginResponse = loginResponse
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendPlayerPosition(Guid accountId, int playerId, int x, int y)
    {

        var playerPosition = new PlayerPosition
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };

        var serverMessage = new ServerMessage
        {
            PlayerPosition = playerPosition
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SpawnPlayer(Guid accountId, int playerId, float x, float y)
    {

        var playerSpawn = new PlayerSpawn
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };


        var serverMessage = new ServerMessage
        {
            PlayerSpawn = playerSpawn
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    // public void SendWorlState(List<Guid> accountsIds, WorldStateUpdate worldStateUpdate)
    // {

    //     var serverWorldMessage = new ServerWorldMessage
    //     {
    //         WorldStateUpdate = worldStateUpdate
    //     };

    //     var serverMessage = new ServerMessage
    //     {
    //         ServerWorldMessage = serverWorldMessage
    //     };

    //     _websocket.SendBroadcast(accountsIds, serverMessage);
    // }
}

internal class AuthResponse
{
    public bool Success { get; set; }
}
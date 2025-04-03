using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Auth;
using Protos.Common;
using Protos.Game.Session;
using VentusServer;

public class MessageSender
{
    private readonly WebSocketServerController _websocket;
    public MessageSender(WebSocketServerController webSocket)
    {
        _websocket = webSocket;
    }
    public void SendAuthResponse(Guid accountId, bool success)
    {
        AuthResponse authResponse = new AuthResponse
        {
            Success = success
        };
        ServerMessage serverMessage = new ServerMessage
        {
            AuthResponse = authResponse
        };
        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendPlayerPosition(Guid accountId, int playerId, float x, float y)
    {

        ServerMessageGameSession serverMessageGameSession = new ServerMessageGameSession();

        PlayerPosition playerPosition = new PlayerPosition
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };
        serverMessageGameSession.PlayerPosition = playerPosition;

        _websocket.SendServerPacketByAccountId(accountId, serverMessageGameSession);
    }
    public void SpawnPlayer(Guid accountId, int playerId, float x, float y)
    {
        ServerMessage serverMessage = new ServerMessage();

        ServerMessageGameSession serverMessageGameSession = new ServerMessageGameSession();
serverMessage.ServerMessageSession = serverMessageGameSession;
        PlayerSpawn playerSpawn = new PlayerSpawn
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };
        serverMessageGameSession.PlayerSpawn = playerSpawn;

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }
}

using System.Net.WebSockets;
using Google.Protobuf;
using Messages.Auth;
using Protos.Game.Common;
using Protos.Game.Session;
using ProtosCommon;
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
        ServerMessage serverMessage = new ServerMessage();
        AuthResponse authResponse = new AuthResponse
        {
            Success = success
        };

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }

    public void SendPlayerPosition(Guid accountId, int playerId, float x, float y)
    {
        ServerMessage serverMessage = new ServerMessage();
        ServerMessageGame serverMessageGame = new ServerMessageGame();
        
        ServerMessageGameSession serverMessageGameSession = new ServerMessageGameSession();

        PlayerPosition playerPosition = new PlayerPosition
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };
        serverMessageGameSession.PlayerPosition = playerPosition;
        serverMessageGame.MessageSession = serverMessageGameSession;
        serverMessage.ServerMessageGame = serverMessageGame;

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }
    public void SpawnPlayer(Guid accountId, int playerId, float x, float y)
    {
        ServerMessage serverMessage = new ServerMessage();
        ServerMessageGame serverMessageGame = new ServerMessageGame();
        ServerMessageGameSession serverMessageGameSession = new ServerMessageGameSession();

        PlayerSpawn playerSpawn = new PlayerSpawn
        {
            PlayerId = playerId,
            X = x,
            Y = y
        };
        serverMessageGameSession.PlayerSpawn = playerSpawn;
        serverMessageGame.MessageSession = serverMessageGameSession;
        serverMessage.ServerMessageGame = serverMessageGame;

        _websocket.SendServerPacketByAccountId(accountId, serverMessage);
    }
}

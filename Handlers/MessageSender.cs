using System.Net.WebSockets;
using Google.Protobuf;
using Messages.Auth;
using ProtosCommon;
using VentusServer;

public class MessageSender
{
    private readonly WebSocketServerController _websocket;
    public MessageSender(WebSocketServerController webSocket)
    {
        _websocket = webSocket;
    }
    public void SendAuthResponse(string userId, bool success)
    {
        ServerMessage serverMessage = new ServerMessage();
        AuthResponse authResponse = new AuthResponse
        {
            Success = success
        };

        _websocket.SendServerPacketByUserID(userId, serverMessage);
    }

}

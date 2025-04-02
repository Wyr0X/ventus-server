
using Protos.Game.Common;
using ProtosCommon;

public class UserMessagePair
{
    public string UserId { get; set; }
    public ClientMessage ClientMessage { get; set; }

    public UserMessagePair(string userId, ClientMessage clientMessage)
    {
        UserId = userId;
        ClientMessage = clientMessage;
    }
    public ClientMessage GetClientMessage()
    {
        return ClientMessage;
    }
    public ClientMessageUnAuth? GetClientUnAuthMessage()
    {
        if (ClientMessage.MessageTypeCase == ClientMessage.MessageTypeOneofCase.MessageUnAuth)
        {
            return ClientMessage.MessageUnAuth;

        }
        ;
        return null;
    }
    public ClientMessageAuth? GetClientAuthMessage()
    {
        if (ClientMessage.MessageTypeCase == ClientMessage.MessageTypeOneofCase.MessageAuth)
        {
            return ClientMessage.MessageAuth;

        }
       ;
        return null;
    }
    public ClientMessageGame? GetClientMessageGame()
    {
        ClientMessageUnAuth? clientMessageUnAuth = GetClientUnAuthMessage();
        if (clientMessageUnAuth != null)
        {
            if (clientMessageUnAuth.MessageTypeCase == ClientMessageUnAuth.MessageTypeOneofCase.ClientMessageGame)
            {
                return clientMessageUnAuth.ClientMessageGame;

            }

        }
       ;
        return null;
    }
}


using Protos.Game.Common;
using ProtosCommon;

public class UserMessagePair
{
    public Guid AccountId { get; set; }
    public ClientMessage ClientMessage { get; set; }

    public UserMessagePair(Guid accountId, ClientMessage clientMessage)
    {
        AccountId = accountId;
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

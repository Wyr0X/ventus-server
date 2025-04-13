
using Google.Protobuf;
using Ventus.Client;
public class UserMessagePair
{
    public Guid AccountId { get; set; }
    public ClientMessage ClientMessage { get; set; }

    public UserMessagePair(Guid accountId, ClientMessage clientMessage)
    {
        AccountId = accountId;
        ClientMessage = clientMessage;
    }
}

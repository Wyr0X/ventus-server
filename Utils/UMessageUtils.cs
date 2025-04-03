
using Google.Protobuf;
public class UserMessagePair
{
    public Guid AccountId { get; set; }
    public IMessage ClientMessage { get; set; }

    public UserMessagePair(Guid accountId, IMessage clientMessage)
    {
        AccountId = accountId;
        ClientMessage = clientMessage;
    }
}

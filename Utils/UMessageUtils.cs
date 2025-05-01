
using Google.Protobuf;
using Ventus.Network.Packets;
public class UserMessagePair
{
    public Guid AccountId { get; set; }
    public ClientPacket PacketType { get; set; }
    public IMessage ClientMessage { get; set; }

    public UserMessagePair(Guid accountId, IMessage clientMessage, ClientPacket packetType)
    {
        AccountId = accountId;
        ClientMessage = clientMessage;
        PacketType = packetType;
    }
}

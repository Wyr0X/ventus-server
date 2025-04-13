using Google.Protobuf;
using Ventus.Server;

public class PrivateChatService : ChatMessageChannel
{



    public void SendPrivateMessage(Guid accountId, string playerName, int playerId, string message, long timestamp, Action<Guid, ServerMessage> sendServerPacketByAccountI)
    {
        this.SendMessageToAccountId(accountId, sendServerPacketByAccountI, playerName, playerId, message, timestamp, ChatChannel.PRIVATE.ToString());
    }

}

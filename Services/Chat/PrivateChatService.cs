using Google.Protobuf;
using Protos.Game.Chat;

public class PrivateChatService : ChatMessageChannel
{



    public void SendPrivateMessage(Guid accountId, ChatSend message, string playerName, Action<Guid, IMessage> sendServerPacketByAccountI)
    {
        this.SendMessageToAccountId(accountId, sendServerPacketByAccountI, playerName, message, ChatChannel.PRIVATE.ToString());
    }

}

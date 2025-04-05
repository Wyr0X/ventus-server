using Google.Protobuf;
using Protos.Game.Chat;

public class GlobalChatService: ChatMessageChannel
{

    

    public void SendGlobalMessage(   List<Guid> accountIdToBroadcast, ChatSend message, string playerName,   Action<Guid, IMessage> sendServerPacketByAccountI)
    {
        this.SendMessageToAccountIds(accountIdToBroadcast, sendServerPacketByAccountI, playerName, message, ChatChannel.GLOBAL.ToString() );
    }

}

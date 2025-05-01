using Google.Protobuf;
using Ventus.Server;

public class GlobalChatService : ChatMessageChannel
{



    public void SendGlobalMessage(List<Guid> accountIdToBroadcast, int playerId, string message, long timestamp, string playerName, Action<Guid, ServerMessage> sendServerPacketByAccountI)
    {
        this.SendMessageToAccountIds(accountIdToBroadcast, sendServerPacketByAccountI, playerName, playerId, message, timestamp, ChatChannel.GLOBAL.ToString());
    }

}

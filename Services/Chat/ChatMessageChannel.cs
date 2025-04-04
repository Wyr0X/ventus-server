using Google.Protobuf;
using Protos.Common;
using Protos.Game.Chat;

public class ChatMessageChannel
{
    public void SendMessageToAccountIds(Guid[] accountsId, Action<Guid, IMessage> sendMessage, string playerName, ChatSend chatSend, ChatChannel channel)
    {
        OutgoingChatMessage outgoingChatMessage = new OutgoingChatMessage
        {
            Message = chatSend.Message,
            PlayerId = chatSend.PlayerId,
            PlayerName = playerName,
            TimestampMs = chatSend.TimestampMs,
            Channel = channel
        };

        foreach (var accountId in accountsId)
        {
            ServerMessageChat serverMessageChat = new ServerMessageChat
            {
                OutgoingChatMessage = outgoingChatMessage
            };
            ServerMessage serverMessage = new ServerMessage
            {
                ServerMessageChat = serverMessageChat
            };

            sendMessage(accountId, serverMessage);
        }
    }
}

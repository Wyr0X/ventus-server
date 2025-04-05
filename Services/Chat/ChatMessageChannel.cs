using Google.Protobuf;
using Protos.Common;
using Protos.Game.Chat;

public class ChatMessageChannel
{
    public void SendMessageToAccountId(Guid accountId, Action<Guid, IMessage> sendMessage, string playerName, ChatSend chatSend, string channel)
    {
        OutgoingChatMessage outgoingChatMessage = new OutgoingChatMessage
        {
            Message = chatSend.Message,
            PlayerId = chatSend.PlayerId,
            PlayerName = playerName,
            TimestampMs = chatSend.TimestampMs,
            Channel = channel
        };
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
    public void SendMessageToAccountIds(List<Guid> accountsId, Action<Guid, IMessage> sendMessage, string playerName, ChatSend chatSend, string channel)
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
            SendMessageToAccountId(accountId, sendMessage, playerName, chatSend, channel);
        }
    }
}
enum ChatChannel
{
    GLOBAL,
    PRIVATE,
    PARTY,
    GUILD,
    SYSTEM,

}
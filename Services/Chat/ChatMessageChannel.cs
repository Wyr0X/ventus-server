using Google.Protobuf;
using Protos.Game.Server.Chat;
using Ventus.Server;

public class ChatMessageChannel
{
    public void SendMessageToAccountId(Guid accountId, Action<Guid, ServerMessage> sendMessage,
    string playerName, int playerId, string message, long timestamp, string channel)
    {
        OutgoingChatMessage outgoingChatMessage = new OutgoingChatMessage
        {
            Message = message,
            PlayerId = playerId,
            PlayerName = playerName,
            TimestampMs = timestamp,
            Channel = channel
        };

        ServerMessage serverMessage = new ServerMessage
        {
            OutgoingChatMessage = outgoingChatMessage
        };
        sendMessage(accountId, serverMessage);
    }
    public void SendMessageToAccountIds(List<Guid> accountsId, Action<Guid, ServerMessage> sendMessage, string playerName,
    int playerId, string message, long timestamp, string channel)
    {

        foreach (var accountId in accountsId)
        {
            SendMessageToAccountId(accountId, sendMessage, playerName, playerId, message, timestamp, channel);
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
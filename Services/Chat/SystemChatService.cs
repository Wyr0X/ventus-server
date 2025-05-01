using Google.Protobuf;
using Protos.Game.Server.Chat;
using Ventus.Server;

public class SystemChatService : ChatMessageChannel
{
    private void Send(
        List<Guid> accountIds,
        string messageText,
        SystemMessageType type,
        Action<Guid, ServerMessage> sendServerPacketByAccountId)
    {


        ServerMessage serverMessage = CreateSystemMessage(messageText, type);
        Log.Log(Log.LogTag.SystemChatService, $"Broadcast message for ${accountIds.Count}  - Message: ${serverMessage}");

        foreach (var accountId in accountIds)
        {
            Log.Log(Log.LogTag.SystemChatService, $"Broadcast message for ${accountId}  - Message: ${serverMessage}");

            sendServerPacketByAccountId(accountId, serverMessage);
        }
    }
    private ServerMessage CreateSystemMessage(string message, SystemMessageType type)
    {
        SystemMessage systemMessage = new SystemMessage
        {
            Message = message,
            Type = type,

        };
        ServerMessage serverMessage = new ServerMessage();
        serverMessage.SystemMessage = systemMessage;
        return serverMessage;
    }
    private void SendSingle(
        Guid accountId,
        string messageText,
        SystemMessageType type,
        Action<Guid, ServerMessage> sendServerPacketByAccountId)
    {
        Send(new List<Guid> { accountId }, messageText, type, sendServerPacketByAccountId);
    }

    // 🎯 INFO
    public void SendInfo(Guid accountId, string text, Action<Guid, ServerMessage> send)
        => SendSingle(accountId, text, SystemMessageType.Info, send);

    public void BroadcastInfo(List<Guid> accountIds, string text, Action<Guid, ServerMessage> send)
        => Send(accountIds, text, SystemMessageType.Info, send);

    // ⚠️ WARNING
    public void SendWarning(Guid accountId, string text, Action<Guid, ServerMessage> send)
        => SendSingle(accountId, text, SystemMessageType.Warning, send);

    public void BroadcastWarning(List<Guid> accountIds, string text, Action<Guid, ServerMessage> send)
        => Send(accountIds, text, SystemMessageType.Warning, send);

    // ❌ ERROR
    public void SendError(Guid accountId, string text, Action<Guid, ServerMessage> send)
        => SendSingle(accountId, text, SystemMessageType.Error, send);

    public void BroadcastError(List<Guid> accountIds, string text, Action<Guid, ServerMessage> send)
        => Send(accountIds, text, SystemMessageType.Error, send);

    // 🔇 MUTE_NOTIFICATION
    public void SendMuteNotification(Guid accountId, string text, Action<Guid, ServerMessage> send)
        => SendSingle(accountId, text, SystemMessageType.MuteNotification, send);

    public void BroadcastMuteNotification(List<Guid> accountIds, string text, Action<Guid, ServerMessage> send)
        => Send(accountIds, text, SystemMessageType.MuteNotification, send);
}

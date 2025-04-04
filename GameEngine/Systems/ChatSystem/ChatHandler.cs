using Protos.Common;
using Protos.Game.Chat;



public class ChatHandler
{
    private readonly ResponseService _responseService;

    private readonly ChatManager _chatManager;
    public ChatHandler( ResponseService responseService, ChatManager chatManager)
    {
        _responseService = responseService;
        _chatManager = chatManager;
    }

  public void HandleChatMessage(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = (ClientMessage)messagePair.ClientMessage;
        ClientMessageChat? messageChat = clientMessage.ClientMessageChat;
        if (messageChat == null) return;
        switch (messageChat.MessageCase)
        {
            case ClientMessageChat.MessageOneofCase.ChatSend:
                _chatManager.HandleChatSend(messagePair.AccountId, messageChat.ChatSend);
                break;

            default:
                Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
                break;
        }
    }
}

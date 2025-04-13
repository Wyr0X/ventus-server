using Ventus.Client;



public class ChatHandler
{
    private readonly ResponseService _responseService;

    private readonly ChatManager _chatManager;
    private readonly MessageDispatcher _messageDispatcher;
    public ChatHandler(MessageDispatcher messageDispatcher, ResponseService responseService, ChatManager chatManager)
    {
        _responseService = responseService;
        _chatManager = chatManager;
        _messageDispatcher = messageDispatcher;
        _messageDispatcher.Subscribe(ClientMessage.PayloadOneofCase.ChatSend, _chatManager.HandleChatSend);
    }

}

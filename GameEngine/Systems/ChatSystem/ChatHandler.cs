using System.Net.WebSockets;
using Protos.Game.Chat;

public class ChatHandler
{
    private readonly ChatManager _chatManager;

    public ChatHandler(ChatManager chatManager)
    {
        _chatManager = chatManager;
    }

    // Función que maneja los mensajes recibidos del cliente
    public void HandleChatMessage(ClientMessageChat chatMessage, WebSocket webSocket)
    {
        switch (chatMessage.MessageTypeCase)
        {
            case ClientMessageChat.MessageTypeOneofCase.SendChatMessage:
                _chatManager.ProcessChatMessage(chatMessage.SendChatMessage, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de chat no reconocido.");
                break;
        }
    }
}

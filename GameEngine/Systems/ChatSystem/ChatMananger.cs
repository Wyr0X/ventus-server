using System.Net.WebSockets;
using Protos.Game.Chat;

public class ChatManager
{
    private readonly ChatLogic _chatLogic;

    public ChatManager(ChatLogic chatLogic)
    {
        _chatLogic = chatLogic;
    }

    // Procesa el mensaje de chat y lo pasa al ChatLogic para su ejecución
    public void ProcessChatMessage(SendChatMessage sendChatMessage, WebSocket webSocket)
    {
        // Validamos el canal (por ejemplo, si el canal es global, privado o grupo)
        if (sendChatMessage.Channel < 0 || sendChatMessage.Channel > 2)
        {
            Console.WriteLine("❌ Canal de chat no válido.");
            return;
        }

        // Llamamos al ChatLogic para manejar el chat
        _chatLogic.HandleChatLogic(sendChatMessage, webSocket);
    }
}

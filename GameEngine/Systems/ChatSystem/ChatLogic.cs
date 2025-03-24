using System.Net.WebSockets;
using Protos.Game.Chat;

public class ChatLogic
{
    // Maneja la lógica del chat
    public void HandleChatLogic(SendChatMessage sendChatMessage, WebSocket webSocket)
    {
        // Enviamos el mensaje a los jugadores
        // Dependiendo del canal, el comportamiento puede ser diferente (global, privado, grupo)
        
        // Ejemplo: Enviar el mensaje a todos los jugadores en el canal global
        switch (sendChatMessage.Channel)
        {
            case 0: // Canal global
                BroadcastToAllPlayers(sendChatMessage);
                break;
            case 1: // Canal privado (solo un mensaje entre dos jugadores)
                SendPrivateMessage(sendChatMessage, webSocket);
                break;
            case 2: // Canal de grupo
                SendGroupMessage(sendChatMessage, webSocket);
                break;
            default:
                Console.WriteLine("❌ Canal no reconocido.");
                break;
        }
    }

    // Envia el mensaje a todos los jugadores en el canal global
    private void BroadcastToAllPlayers(SendChatMessage sendChatMessage)
    {
        // Lógica para enviar el mensaje a todos los jugadores conectados
        Console.WriteLine($"📢 [Global] {sendChatMessage.Message}");
        // Aquí, enviaríamos el mensaje a todos los jugadores (usualmente mediante WebSocket)
    }

    // Envia un mensaje privado entre jugadores
    private void SendPrivateMessage(SendChatMessage sendChatMessage, WebSocket webSocket)
    {
        // Aquí debes obtener el jugador objetivo y enviar el mensaje directamente
        Console.WriteLine($"💬 [Privado] {sendChatMessage.Message}");
        // Usualmente esto implica identificar al jugador y su WebSocket específico
    }

    // Envia un mensaje a todos los jugadores de un grupo
    private void SendGroupMessage(SendChatMessage sendChatMessage, WebSocket webSocket)
    {
        // Lógica para enviar el mensaje a todos los miembros de un grupo
        Console.WriteLine($"👥 [Grupo] {sendChatMessage.Message}");
        // Esto implicaría identificar los miembros del grupo y enviarles el mensaje
    }
}

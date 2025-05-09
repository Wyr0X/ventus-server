using System.Net.WebSockets;
using Google.Protobuf;

public class ResponseService
{

    public async Task SendMessageAsync(WebSocket webSocket, IMessage message)
    {
        // Crear un ServerMessage y empaquetar el mensaje específico (AuthResponse o ChatMessage)

        // Serializar y enviar el mensaje empaquetado
        var messageBytes = message.ToByteArray();
        await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Binary, true, CancellationToken.None).ConfigureAwait(false);
    }

    public void SendBroadcast(WebSocket[] webSockets, IMessage message)
    {

        foreach (var webSocket in webSockets)
        {
            _ = SendMessageAsync(webSocket, message);
        }
    }
}

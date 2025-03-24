using System.Net.WebSockets;
using Google.Protobuf;
using Messages.Auth;
using ProtosCommon;

public class ResponseService
{
 
    public async Task SendMessageAsync(WebSocket webSocket, IMessage message)
        {
            // Crear un ServerMessage y empaquetar el mensaje específico (AuthResponse o ChatMessage)
            var serverMessage = new ServerMessage();

            switch (message)
            {
                case AuthResponse authResponse:
                    serverMessage.AuthResponse = authResponse;
                    break;

                // Puedes añadir más casos aquí según sea necesario para otros tipos de respuesta

                default:
                    Console.WriteLine("❌ Tipo de mensaje no manejado para envío.");
                    return;
            }

            // Serializar y enviar el mensaje empaquetado
            var messageBytes = serverMessage.ToByteArray();
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
}

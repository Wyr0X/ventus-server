using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Game.World;

public class WorldLogic
{
    // Lógica para actualizar la ubicación de un jugador en el mundo
    public void UpdatePlayerLocation(ClientMessageLocationUpdate message, WebSocket webSocket)
    {
        string playerId = message.PlayerId;
        float x = message.X;
        float y = message.Y;

        // Aquí puedes actualizar la ubicación del jugador en tu base de datos o lógica del juego.
        Console.WriteLine($"🔹 Jugador {playerId} se ha movido a las coordenadas ({x}, {y}).");

        // Responder con el estado actualizado del mundo si es necesario
        var response = new ServerMessageWorld
        {
            MessageWorldStateUpdate = new ServerMessageWorldStateUpdate
            {
                WorldState = $"Jugador {playerId} ha sido movido a ({x}, {y})"
            }
        };

        SendResponse(response, webSocket);
    }

    // Lógica para procesar una interacción con el entorno del mundo
    public void ProcessEnvironmentInteraction(ClientMessageEnvironmentInteraction message, WebSocket webSocket)
    {
        string playerId = message.PlayerId;
        string action = message.Action;

        // Lógica para manejar la acción realizada en el entorno
        Console.WriteLine($"🔹 Jugador {playerId} interactuó con el entorno realizando la acción: {action}");

        // Responder con el resultado de la interacción si es necesario
        var response = new ServerMessageWorld
        {
            MessageWorldInteractionResult = new ServerMessageWorldInteractionResult
            {
                Success = true,
                Message = $"Acción '{action}' realizada exitosamente."
            }
        };

        SendResponse(response, webSocket);
    }

    // Método genérico para enviar respuestas (deberás ajustarlo según tu implementación real)
    private void SendResponse(ServerMessageWorld response, WebSocket webSocket)
    {
        // Aquí envías la respuesta de vuelta al cliente a través del WebSocket
        var responseBytes = response.ToByteArray();
        webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}

using System.Net.WebSockets;
using Protos.Game.Movement;
using ProtosCommon;

public class MovementLogic
{
    // Lógica para mover al jugador
    public void MovePlayerLogic(UserMessagePair messagePair)
    {
        // Lógica para mover al jugador (ejemplo simple)
        ClientMessage clientMessage = messagePair.ClientMessage;
        MoveRequest moveRequest = clientMessage.MessageOauth.ClientMessageGame.MessageMovement.MoveRequest;
        Console.WriteLine($"🚶‍♂️ Jugador moviéndose a la posición ({moveRequest.X}, {moveRequest.Y})");

        // Aquí validaríamos si el movimiento es válido, sin colisiones o restricciones.

        // Crear la respuesta del movimiento
        var moveResponse = new MoveResponse
        {
            Success = true,
            Reason = "Movimiento exitoso"
        };

        // Enviar la respuesta de movimiento al cliente
        SendMovementResult(moveResponse, webSocket);

        // Notificar a los demás jugadores que el jugador se ha movido
        var playerMoved = new PlayerMoved
        {
            PlayerId = 1,  // ID del jugador
            X = moveRequest.X,
            Y = moveRequest.Y
        };

        // Aquí enviaríamos el movimiento al resto de jugadores
        BroadcastMovement(playerMoved);
    }

    // Lógica para cambiar la dirección del jugador
    public void ChangePlayerDirectionLogic(ChangeDirectionRequest changeDirectionRequest, WebSocket webSocket)
    {
        // Lógica para cambiar la dirección (ejemplo simple)
        Console.WriteLine($"🧭 Jugador cambiando la dirección a {changeDirectionRequest.Direction}");

        // Crear la respuesta de cambio de dirección
        var playerDirectionChanged = new PlayerDirectionChanged
        {
            PlayerId = 1,  // ID del jugador
            Direction = changeDirectionRequest.Direction
        };

        // Aquí enviaríamos el resultado del cambio de dirección al cliente
        SendMovementResult(playerDirectionChanged, webSocket);
    }

    // Método para enviar el resultado de la acción de movimiento al cliente
    private void SendMovementResult(object movementResult, WebSocket webSocket)
    {
        // Aquí enviaríamos el resultado de la ejecución del movimiento al cliente a través del WebSocket
        Console.WriteLine($"📢 Resultado de la acción de movimiento: {movementResult.ToString()}");
    }

    // Método para notificar a otros jugadores que un jugador se ha movido
    private void BroadcastMovement(PlayerMoved playerMoved)
    {
        // Aquí se enviaría a todos los jugadores conectados (excepto al jugador que se movió)
        Console.WriteLine($"📣 Jugador {playerMoved.PlayerId} se ha movido a ({playerMoved.X}, {playerMoved.Y})");
    }
}

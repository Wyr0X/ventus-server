using System.Net.WebSockets;
using Protos.Game.Movement;

public class MovementManager
{
    private readonly MovementLogic _movementLogic;

    public MovementManager(MovementLogic movementLogic)
    {
        _movementLogic = movementLogic;
    }

    // Procesa la solicitud de movimiento
    public void ProcessMoveRequest(MoveRequest moveRequest, WebSocket webSocket)
    {
        if (moveRequest.X == 0 && moveRequest.Y == 0)
        {
            Console.WriteLine("❌ Movimiento inválido (sin desplazamiento).");
            return;
        }

        _movementLogic.MovePlayerLogic(moveRequest, webSocket);
    }

    // Procesa la solicitud de cambio de dirección
    public void ProcessChangeDirectionRequest(ChangeDirectionRequest changeDirectionRequest, WebSocket webSocket)
    {
        if (changeDirectionRequest.Direction < 0 || changeDirectionRequest.Direction > 3)
        {
            Console.WriteLine("❌ Dirección inválida.");
            return;
        }

        _movementLogic.ChangePlayerDirectionLogic(changeDirectionRequest, webSocket);
    }
}

using System.Net.WebSockets;
using Protos.Game.Movement;

public class MovementHandler
{
    private readonly MovementManager _movementManager;

    public MovementHandler(MovementManager movementManager)
    {
        _movementManager = movementManager;
    }

    // Función que maneja los mensajes de movimiento recibidos desde el cliente
    public void HandleMovementMessage(ClientMessageMovement movementMessage, WebSocket webSocket)
    {
        switch (movementMessage.MessageTypeCase)
        {
            case ClientMessageMovement.MessageTypeOneofCase.MoveRequest:
                _movementManager.ProcessMoveRequest(movementMessage.MoveRequest, webSocket);
                break;
            case ClientMessageMovement.MessageTypeOneofCase.ChangeDirectionRequest:
                _movementManager.ProcessChangeDirectionRequest(movementMessage.ChangeDirectionRequest, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
                break;
        }
    }
}

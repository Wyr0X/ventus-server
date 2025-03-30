using System.Net.WebSockets;
using Protos.Game.Common;
using Protos.Game.Movement;
using ProtosCommon;

public class MovementHandler
{
    private readonly MovementManager _movementManager;

    public MovementHandler(MovementManager movementManager)
    {
        _movementManager = movementManager;;
    }

    // Función que maneja los mensajes de movimiento recibidos desde el cliente
    public void HandleMovementMessage(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = messagePair.ClientMessage;
        ClientMessageMovement movementMessage = clientMessage.MessageOauth.ClientMessageGame.MessageMovement;
        switch (movementMessage.MessageTypeCase)
        {
            case ClientMessageMovement.MessageTypeOneofCase.MoveRequest:
                _movementManager.HandleMoveRequest(messagePair);
                break;
            
            default:
                Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
                break;
        }
    }
}

using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Game.Movement;

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
        ClientMessageMovement? movementMessage = (ClientMessageMovement?)messagePair.ClientMessage;
        if (movementMessage == null) return;
        switch (movementMessage.MessageTypeCase)
        {
            case ClientMessageMovement.MessageTypeOneofCase.PlayerInput:
                _movementManager.HandlePlayerInput(messagePair.AccountId, movementMessage.PlayerInput);
                break;

            default:
                Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
                break;
        }
    }
}

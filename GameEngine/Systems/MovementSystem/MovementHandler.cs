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
        ClientMessageGame? clientMessageGame = messagePair.GetClientMessageGame();
        if (clientMessageGame == null) return;
        ClientMessageMovement movementMessage = clientMessageGame.MessageMovement;
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

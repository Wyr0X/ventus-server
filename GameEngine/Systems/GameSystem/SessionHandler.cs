using System.Net.WebSockets;
using Protos.Common;
using Protos.Game.Movement;
using Protos.Game.Session;

public class SessionHandler
{

    private readonly SessionManager _sessionManager;

    public SessionHandler(SessionManager sessionManager)
    {
        _sessionManager = sessionManager;


    }
    // Función que maneja los mensajes de movimiento recibidos desde el cliente

    public void HandleSessionMessage(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = (ClientMessage)messagePair.ClientMessage;

        ClientMessageGameSession sessionMessage = clientMessage.ClientMessageSession;
        switch (sessionMessage.MessageTypeCase)
        {
            case ClientMessageGameSession.MessageTypeOneofCase.PlayerJoin:
                _sessionManager.HandlePlayerJoin(messagePair);
                break;

            default:
                Console.WriteLine("❌ Tipo de mensaje de movimiento no reconocido.");
                break;
        }
    }
}

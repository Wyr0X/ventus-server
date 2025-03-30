using System.Net.WebSockets;
using Protos.Game.Common;
using Protos.Game.Movement;
using Protos.Game.Session;
using ProtosCommon;

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
        ClientMessage clientMessage = messagePair.ClientMessage;
        ClientMessageGameSession sessionMessage = clientMessage.MessageOauth.ClientMessageGame.MessageSession;
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

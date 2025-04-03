using System.Net.WebSockets;
using FirebaseAdmin.Messaging;
using Google.Protobuf;
using Protos.Common;
using Protos.Game.Session;
using VentusServer;

public class MessageHandler
{
    private readonly AuthHandler _authHandler;
    private readonly SessionHandler _sessionHandler;

    public MessageHandler(AuthHandler authHandler, SessionHandler sessionHandler)
    {
        _authHandler = authHandler;
        _sessionHandler = sessionHandler;

    }

    public void HandleIncomingMessage(UserMessagePair messagePair)
    {
        try
        {
            ClientMessage clientMessage = (ClientMessage)messagePair.ClientMessage;

            Console.WriteLine($"🔹 Tipo de mensaje recibido: {clientMessage.GetType().Name}");
            if (clientMessage.MessageCase == ClientMessage.MessageOneofCase.ClientMessageSession)
            {
                _sessionHandler.HandleSessionMessage(messagePair);

            }
            
            // else if (clientMessage is MessageUnAuth unAuthMessage)
            // {
            //     HandleOAuthMessage(unAuthMessage, messagePair);
            // }
            else
            {
                Console.WriteLine("❌ Mensaje recibido sin un tipo válido.");
            }
        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }



    
}

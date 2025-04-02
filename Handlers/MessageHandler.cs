using System.Net.WebSockets;
using FirebaseAdmin.Messaging;
using Google.Protobuf;
using Protos.Game.Common;
using ProtosCommon;
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


            ClientMessage clientMessage = messagePair.ClientMessage;

            Console.WriteLine($"🔹 Tipo de mensaje recibido: {clientMessage.MessageTypeCase}, {ClientMessage.MessageTypeOneofCase.MessageUnAuth}");
            switch (clientMessage.MessageTypeCase)
            {
                case ClientMessage.MessageTypeOneofCase.MessageAuth:
                    HandleAuthMessage(messagePair);
                    break;
                case ClientMessage.MessageTypeOneofCase.MessageUnAuth:

                    HandleOAuthMessage(messagePair);
                    break;
    
                default:
                    Console.WriteLine("❌ Mensaje recibido sin un tipo válido. 1");
                    break;
            }
        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
    public void HandleAuthMessage(UserMessagePair messagePair)
    {
        try
        {
            ClientMessage clientMessage = messagePair.ClientMessage;
            var clientMessageAuth = clientMessage.MessageAuth;
            switch (clientMessageAuth.MessageTypeCase)
            {
                default:
                    Console.WriteLine("❌ Mensaje recibido sin un tipo válido.");
                    break;
            }

        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
    public void HandleOAuthMessage(UserMessagePair messagePair)
    {
        try
        {
            ClientMessage clientMessage = messagePair.ClientMessage;
            var clientMessageOAuth = clientMessage.MessageUnAuth;
            switch (clientMessageOAuth.MessageTypeCase)
            {
                case ClientMessageUnAuth.MessageTypeOneofCase.ClientMessageGame:
                    HandleMessageGame(messagePair);
                    break;
                default:
                    Console.WriteLine("❌ Mensaje recibido sin un tipo válido.");
                    break;
            }

        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
    public void HandleMessageGame(UserMessagePair messagePair)
    {
        try
        {
            ClientMessage clientMessage = messagePair.ClientMessage;
            ClientMessageGame clientMessageGame = clientMessage.MessageUnAuth.ClientMessageGame;
            switch (clientMessageGame.MessageTypeCase)
            {
                /*case ClientMessageGame.MessageTypeOneofCase.MessageMovement:
                    _movementHandler.HandleMovementMessage(messagePair );
                    break;*/
                case ClientMessageGame.MessageTypeOneofCase.MessageSession:
                    _sessionHandler.HandleSessionMessage(messagePair);
                    break;
                default:
                    Console.WriteLine("❌ Mensaje recibido sin un tipo válido.");
                    break;
            }

        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
}

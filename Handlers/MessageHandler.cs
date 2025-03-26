using System.Net.WebSockets;
using FirebaseAdmin.Messaging;
using Google.Protobuf;
using Protos.Game.Common;
using ProtosCommon;
using VentusServer;

public class MessageHandler
{
    private readonly AuthHandler _authHandler;
    private readonly MovementHandler _movementHandler;

    public MessageHandler(AuthHandler authHandler, MovementHandler movementHandler)
    {
        _authHandler = authHandler;
        _movementHandler = movementHandler;

    }

    public void HandleIncomingMessage(UserMessagePair messagePair)
    {
        try
        {


            ClientMessage clientMessage = messagePair.ClientMessage;

            Console.WriteLine($"🔹 Tipo de mensaje recibido: {clientMessage.MessageTypeCase}");
            switch (clientMessage.MessageTypeCase)
            {
                case ClientMessage.MessageTypeOneofCase.MessageAuth:
                    HandleAuthMessage(messagePair);
                    break;
                case ClientMessage.MessageTypeOneofCase.MessageOauth:
                    HandleOAuthMessage(messagePair);
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
            var clientMessageOAuth = clientMessage.MessageOauth;
            switch (clientMessageOAuth.MessageTypeCase)
            {
                case ClientMessageOAuth.MessageTypeOneofCase.ClientMessageGame:
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
            ClientMessageGame clientMessageGame = clientMessage.MessageOauth.ClientMessageGame;
            switch (clientMessageGame.MessageTypeCase)
            {
                case ClientMessageGame.MessageTypeOneofCase.MessageMovement:
                    _movementHandler.HandleMovementMessage(message.MessageMovement, );
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

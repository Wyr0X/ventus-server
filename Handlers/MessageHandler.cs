using System.Net.WebSockets;
using Google.Protobuf;
using Protos.Game;
using ProtosCommon;
using VentusServer;

public class MessageHandler
{
    private readonly AuthHandler _authHandler;
    private readonly MovementHandler _movementHandler;
    private readonly CombatHandler _combatHandler;
    private readonly ChatHandler _chatHandler;
    private readonly ItemsHandler _itemsHandler;
    private readonly StatsHandler _statsHandler;
    private readonly CommandHandler _commandHandler;

    private readonly WorldHandler _worldHandler;

    public MessageHandler(AuthHandler authHandler, MovementHandler movementHandler, CombatHandler combatHandler,
                         ChatHandler chatHandler, ItemsHandler itemsHandler, StatsHandler statsHandler, 
                         CommandHandler commandHandler, WorldHandler worldHandler)
    {
        _authHandler = authHandler;
        _movementHandler = movementHandler;
        _combatHandler = combatHandler;
        _chatHandler = chatHandler;
        _itemsHandler = itemsHandler;
        _statsHandler = statsHandler;
        _commandHandler = commandHandler;
        _worldHandler = worldHandler;

    }

    public void HandleIncomingMessage(byte[] receivedBytes, WebSocket webSocket, string? userId)
    {
        try
        {

            Console.WriteLine($"🔹 Recibiendo mensaje del WebSocket. Tamaño: {receivedBytes} bytes.");

            var clientMessage = ClientMessage.Parser.ParseFrom(receivedBytes);
            Console.WriteLine($"🔹 Tipo de mensaje recibido: {clientMessage.MessageTypeCase}");
            switch (clientMessage.MessageTypeCase)
            {
                case ClientMessage.MessageTypeOneofCase.MessageAuth:
                    HandleAuthMessage(clientMessage.MessageAuth, webSocket);
                    break;
                case ClientMessage.MessageTypeOneofCase.MessageOauth:
                    HandleOAuthMessage(clientMessage.MessageOauth, webSocket);
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
    public void HandleAuthMessage(ClientMessageAuth message, WebSocket webSocket)
    {
        try
        {
            switch (message.MessageTypeCase)
            {
                case ClientMessageAuth.MessageTypeOneofCase.AuthRequest:
                    _ = _authHandler.HandleAuthMessage(message.AuthRequest, webSocket);
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
    public void HandleOAuthMessage(ClientMessageOAuth message, WebSocket webSocket)
    {
        try
        {
            switch (message.MessageTypeCase)
            {
                case ClientMessageOAuth.MessageTypeOneofCase.ClientMessageGame:
                    HandleGameMessage(message.ClientMessageGame, webSocket);
                    break;
                default:
                    Console.WriteLine("❌ Mensaje de OAuth no reconocido.");
                    break;
            }
        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
    public void HandleGameMessage(ClientMessageGame message, WebSocket webSocket)
    {
        try
        {
            switch (message.MessageTypeCase)
            {
                case ClientMessageGame.MessageTypeOneofCase.MessageMovement:
                    _movementHandler.HandleMovementMessage(message.MessageMovement, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageCombat:
                    _combatHandler.HandleCombatMessage(message.MessageCombat, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageChat:
                    _chatHandler.HandleChatMessage(message.MessageChat, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageItems:
                    _itemsHandler.HandleItemsMessage(message.MessageItems, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageStats:
                    _statsHandler.HandleStatsMessage(message.MessageStats, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageCommand:
                    _commandHandler.HandleCommandHandler(message.MessageCommand, webSocket);
                    break;
                case ClientMessageGame.MessageTypeOneofCase.MessageWorld:
                    _worldHandler.HandleWorldHandler(message.MessageWorld, webSocket);
                    break;
                default:
                    Console.WriteLine("❌ Tipo de mensaje de juego no reconocido.");
                    break;
            }
        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje de juego.");
        }
    }
}

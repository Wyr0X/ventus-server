using System.Net.WebSockets;
using Google.Protobuf;
using ProtosCommon;
using VentusServer;

public class MessageHandler
{
    private readonly AuthHandler _authHandler;

    public MessageHandler(AuthHandler authHandler)
    {
        _authHandler = authHandler;
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
            Console.WriteLine("Completar");
        
        }
        catch (InvalidProtocolBufferException)
        {
            Console.WriteLine("❌ No se pudo deserializar el mensaje.");
        }
    }
}

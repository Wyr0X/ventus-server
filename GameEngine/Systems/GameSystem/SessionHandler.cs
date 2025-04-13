using System.Net.WebSockets;
using Ventus.Client;

public class SessionHandler
{

    private readonly SessionManager _sessionManager;
    private readonly MessageDispatcher _messageDispatcher;
    public SessionHandler(MessageDispatcher messageDispatcher, SessionManager sessionManager)
    {
        Console.WriteLine("Entra acaaaaaaaaaaaa 7");

        _sessionManager = sessionManager;

        _messageDispatcher = messageDispatcher;
        Console.WriteLine($"Entra acaaaaaaaaaaaa 5 ${_messageDispatcher}");

        _messageDispatcher.Subscribe(ClientMessage.PayloadOneofCase.PlayerJoin, _sessionManager.HandlePlayerJoin);


    }

}

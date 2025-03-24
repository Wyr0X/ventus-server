using System.Net.WebSockets;
using Protos.Game.Command;

public class CommandHandler
{
    private readonly CommandManager _commandManager;

    public CommandHandler(CommandManager commandManager)
    {
        _commandManager = commandManager;
    }

    // Función que maneja los mensajes de comando recibidos desde el cliente
    public void HandleCommandMessage(ClientMessageCommand commandMessage, WebSocket webSocket)
    {
        switch (commandMessage.MessageTypeCase)
        {
            case ClientMessageCommand.MessageTypeOneofCase.MessageExecuteCommand:
                _commandManager.ProcessExecuteCommand(commandMessage.MessageExecuteCommand, webSocket);
                break;
            case ClientMessageCommand.MessageTypeOneofCase.MessageChangeState:
                _commandManager.ProcessChangeState(commandMessage.MessageChangeState, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de comando no reconocido.");
                break;
        }
    }
}

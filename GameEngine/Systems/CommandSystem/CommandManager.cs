using System.Net.WebSockets;
using Protos.Game.Command;

public class CommandManager
{
    private readonly CommandLogic _commandLogic;

    public CommandManager(CommandLogic commandLogic)
    {
        _commandLogic = commandLogic;
    }

    // Procesa la solicitud de ejecución de un comando
    public void ProcessExecuteCommand(ClientMessageExecuteCommand executeCommand, WebSocket webSocket)
    {
        // Validación del comando
        if (string.IsNullOrEmpty(executeCommand.CommandName))
        {
            Console.WriteLine("❌ El nombre del comando es inválido.");
            return;
        }

        // Llamamos a la lógica de comandos para ejecutar el comando
        _commandLogic.ExecuteCommandLogic(executeCommand, webSocket);
    }

    // Procesa la solicitud de cambio de estado del jugador
    public void ProcessChangeState(ClientMessageChangeState changeState, WebSocket webSocket)
    {
        // Validación del cambio de estado
        if (string.IsNullOrEmpty(changeState.PlayerId) || string.IsNullOrEmpty(changeState.NewState))
        {
            Console.WriteLine("❌ Datos de cambio de estado inválidos.");
            return;
        }

        // Llamamos a la lógica de comandos para cambiar el estado del jugador
        _commandLogic.ChangePlayerStateLogic(changeState, webSocket);
    }
}

using System.Net.WebSockets;
using Protos.Game.Command;

public class CommandLogic
{
    // Lógica de ejecución de un comando
    public void ExecuteCommandLogic(ClientMessageExecuteCommand executeCommand, WebSocket webSocket)
    {
        // Ejemplo de ejecución del comando: puede ser ejecutar una acción en el juego
        bool success = false;
        string message = "Comando no reconocido.";

        // Lógica para determinar qué hacer dependiendo del nombre del comando
        if (executeCommand.CommandName == "saludar")
        {
            success = true;
            message = "¡Hola, jugador!";
        }
        else if (executeCommand.CommandName == "mover")
        {
            success = true;
            message = "Movimiento realizado.";
        }

        // Crear el resultado del comando y enviarlo al jugador
        var commandResult = new ServerMessageCommandResult
        {
            Success = success,
            Message = message
        };

        // Aquí enviaríamos el resultado al cliente
        SendCommandResult(commandResult, webSocket);
    }

    // Lógica para cambiar el estado del jugador
    public void ChangePlayerStateLogic(ClientMessageChangeState changeState, WebSocket webSocket)
    {
        // Lógica para cambiar el estado del jugador (ejemplo simple)
        var success = ChangeStateForPlayer(changeState.PlayerId, changeState.NewState);

        // Crear el resultado del cambio de estado
        var commandResult = new ServerMessageCommandResult
        {
            Success = success,
            Message = success ? "Estado cambiado correctamente." : "Error al cambiar el estado."
        };

        // Aquí enviaríamos el resultado al cliente
        SendCommandResult(commandResult, webSocket);
    }

    // Método para cambiar el estado de un jugador (ejemplo simple)
    private bool ChangeStateForPlayer(string playerId, string newState)
    {
        // Aquí va la lógica real para cambiar el estado del jugador
        Console.WriteLine($"Cambiando estado del jugador {playerId} a {newState}");
        return true;
    }

    // Envía el resultado del comando al cliente
    private void SendCommandResult(ServerMessageCommandResult result, WebSocket webSocket)
    {
        // Aquí enviaríamos el resultado de la ejecución del comando al cliente a través del WebSocket
        Console.WriteLine($"📢 Resultado del comando: {result.Message}");
    }
}

using System.Net.WebSockets;
using Protos.Game.World;

public class WorldManager
{
    private readonly WorldLogic _worldSystemLogic;

    public WorldManager(WorldLogic worldSystemLogic)
    {
        _worldSystemLogic = worldSystemLogic;
    }

    public void HandleLocationUpdate(ClientMessageLocationUpdate message, WebSocket webSocket)
    {
        // Delegamos el manejo de la actualización de ubicación a la lógica del sistema
        _worldSystemLogic.UpdatePlayerLocation(message, webSocket);
    }

    public void HandleEnvironmentInteraction(ClientMessageEnvironmentInteraction message, WebSocket webSocket)
    {
        // Delegamos el manejo de la interacción con el entorno a la lógica del sistema
        _worldSystemLogic.ProcessEnvironmentInteraction(message, webSocket);
    }
}

using System.Net.WebSockets;
using Protos.Game.World;

public class WorldHandler
{
    private readonly WorldManager _worldSystemManager;

    public WorldHandler(WorldManager worldSystemManager)
    {
        _worldSystemManager = worldSystemManager;
    }

    public void HandleWorldHandler(ClientMessageWorld worldMessages, WebSocket webSocket)
    {
        // Lógica para manejar los mensajes de mundo
        switch (worldMessages.MessageTypeCase)
        {
            case ClientMessageWorld.MessageTypeOneofCase.MessageLocationUpdate:
                _worldSystemManager.HandleLocationUpdate(worldMessages.MessageLocationUpdate, webSocket);
                break;

            case ClientMessageWorld.MessageTypeOneofCase.MessageEnvironmentInteraction:
                _worldSystemManager.HandleEnvironmentInteraction(worldMessages.MessageEnvironmentInteraction, webSocket);
                break;

            default:
                Console.WriteLine("❌ Mensaje de mundo no reconocido.");
                break;
        }
    }
}

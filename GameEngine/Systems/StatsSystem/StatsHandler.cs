using System.Net.WebSockets;
using Protos.Game.Stats;

public class StatsHandler
{
    private readonly StatsManager _statsManager;

    public StatsHandler(StatsManager statsManager)
    {
        _statsManager = statsManager;
    }

    // Función que maneja los mensajes de estadísticas recibidos desde el cliente
    public void HandleStatsMessage(ClientMessageStats statsMessage, WebSocket webSocket)
    {
        switch (statsMessage.MessageTypeCase)
        {
            case ClientMessageStats.MessageTypeOneofCase.RequestStatsUpdate:
                _statsManager.ProcessStatsUpdateRequest(statsMessage.RequestStatsUpdate, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de estadísticas no reconocido.");
                break;
        }
    }
}

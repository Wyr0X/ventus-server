using System.Net.WebSockets;
using Protos.Game.Stats;

public class StatsManager
{
    private readonly StatsLogic _statsLogic;

    public StatsManager(StatsLogic statsLogic)
    {
        _statsLogic = statsLogic;
    }

    // Procesa la solicitud de actualización de estadísticas
    public void ProcessStatsUpdateRequest(RequestStatsUpdate requestStatsUpdate, WebSocket webSocket)
    {
        // Aquí podríamos validar si la solicitud es correcta y si el jugador tiene permiso para actualizar sus estadísticas.

        _statsLogic.UpdatePlayerStatsLogic(requestStatsUpdate, webSocket);
    }
}

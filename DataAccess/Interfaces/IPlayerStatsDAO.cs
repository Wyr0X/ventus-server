using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IPlayerStatsDAO
    {
        // Obtener las estadísticas de un jugador por su ID
        Task<PlayerStatsModel?> GetPlayerStatsByIdAsync(int playerId);

        // Obtener estadísticas de jugador por su nombre
        Task<PlayerStatsModel?> GetPlayerStatsByNameAsync(string name);

        // Obtener todas las estadísticas de los jugadores
        Task<List<PlayerStatsModel>> GetAllPlayerStatsAsync();

        // Obtener las estadísticas de un jugador por su ID de cuenta
        Task<List<PlayerStatsModel>> GetPlayerStatsByAccountIdAsync(Guid accountId);

        // Crear estadísticas para un nuevo jugador
        Task<PlayerStatsModel> CreatePlayerStatsAsync(int playerId);

        // Actualizar las estadísticas de un jugador
        Task SavePlayerStatsAsync(PlayerStatsModel playerStats);

        // Eliminar las estadísticas de un jugador
        Task<bool> DeletePlayerStatsAsync(int playerId);

        // Verificar si las estadísticas de un jugador existen
        Task<bool> PlayerStatsExistsAsync(int playerId);
    }
}

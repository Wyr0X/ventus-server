using Game.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IPlayerLocationDAO
    {
        Task<PlayerLocationModel?> GetPlayerLocationAsync(int playerId);
        Task SavePlayerLocationAsync(PlayerLocationModel location);
        Task DeletePlayerLocationAsync(int playerId);
        Task CreatePlayerLocationAsync(PlayerLocationModel location);
        Task<List<int>> GetPlayesrIdsByWorldIdAsync(int worldId);
        Task<List<int>> GetPlayesrIdsByMapIdAsync(int mapId);
    }
}

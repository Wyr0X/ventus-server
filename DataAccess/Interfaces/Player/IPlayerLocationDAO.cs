using Game.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IPlayerLocationDAO
    {
        Task<PlayerLocation?> GetPlayerLocationAsync(int playerId);
        Task SavePlayerLocationAsync(PlayerLocation location);
        Task DeletePlayerLocationAsync(int playerId);
        Task CreatePlayerLocationAsync(PlayerLocation location);
    }
}

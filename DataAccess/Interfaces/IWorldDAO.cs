using Game.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IWorldDAO
    {
        Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements);
        Task<WorldModel?> GetWorldByIdAsync(int worldId);
        Task<List<WorldModel>> GetAllWorldsAsync();
        Task SaveWorldAsync(WorldModel world);
        Task DeleteWorldAsync(int worldId);
    }
}

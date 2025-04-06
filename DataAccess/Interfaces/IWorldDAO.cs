using Game.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IWorldDAO
    {
        Task<WorldModel?> GetWorldByIdAsync(int id);
        Task<IEnumerable<WorldModel>> GetAllWorldsAsync();
        Task CreateWorldAsync(WorldModel world);
        Task UpdateWorldAsync(WorldModel world);
        Task DeleteWorldAsync(int id);
    }
}

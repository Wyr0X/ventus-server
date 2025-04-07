using Game.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IMapDAO
    {
        Task<MapModel?> GetMapByIdAsync(int id);
        Task<IEnumerable<MapModel>> GetAllMapsAsync();
        Task<MapModel>? CreateMapAsync(MapModel map);
        Task UpdateMapAsync(MapModel map);
        Task<bool> DeleteMapAsync(int id); // Cambiado a bool para indicar éxito o fracaso

    }
}

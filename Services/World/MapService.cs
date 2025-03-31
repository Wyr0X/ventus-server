using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class MapService
    {
        private PostgresMapDAO _mapDAO;

        public MapService(PostgresMapDAO mapDAO)
        {
            _mapDAO = mapDAO;
        }
        public async Task<MapModel?> CreateMapAsync(string name, int minLevel, int maxPlayers, int worldId)
        {
            return await _mapDAO.CreateMapAsync(name, minLevel, maxPlayers, worldId);
        }
        public async Task<MapModel?> GetMapByIdAsync(int mapId)
        {
            return await _mapDAO.GetMapByIdAsync(mapId);
        }

        public async Task<List<MapModel>> GetAllMapsAsync()
        {
            return await _mapDAO.GetAllMapsAsync();
        }

        public async Task SaveMapAsync(MapModel map)
        {
            await _mapDAO.SaveMapAsync(map);
        }

        public async Task DeleteMapAsync(int mapId)
        {
            await _mapDAO.DeleteMapAsync(mapId);

        }
    }
}

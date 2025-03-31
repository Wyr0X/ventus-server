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
        private PlayerLocationService _playerLocationService;



        public MapService(PostgresMapDAO mapDAO, PlayerLocationService playerLocationService)
        {
            _mapDAO = mapDAO;
            _playerLocationService = playerLocationService;
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

        public async Task RemovePlayerFromMap(int playerId)
        {
            PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(playerId);
            if (playerLocation != null)
            {
                MapModel? map = playerLocation.Map;

                if (map != null)
                {
                    map.RemovePlayer(playerId);
                    await _mapDAO.SaveMapAsync(map);
                }
            }

        }
    }
}

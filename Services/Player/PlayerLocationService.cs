using System;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class PlayerLocationService
    {
        private readonly PostgresPlayerLocationDAO _playerLocationDAO;
        private readonly MapService _mapService;
        private readonly WorldService _worldService;

        public PlayerLocationService(PostgresPlayerLocationDAO playerLocationDAO, MapService mapService, WorldService worldService)
        {
            _playerLocationDAO = playerLocationDAO;
            _mapService = mapService;
            _worldService = worldService;
        }

        // Obtener la ubicación de un jugador
        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            try
            {
                return await _playerLocationDAO.GetPlayerLocationAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener la ubicación del jugador: {ex.Message}");
                return null;
            }
        }

        // Guardar la ubicación de un jugador
        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            try
            {
                await _playerLocationDAO.SavePlayerLocationAsync(location);
                Console.WriteLine("✅ Ubicación del jugador guardada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar la ubicación del jugador: {ex.Message}");
            }
        }


        public async Task CreatePlayerLocation(PlayerLocation playerLocation)
        {
            try
            {
                await _playerLocationDAO.CreatePlayerLocationAsync(playerLocation);
                Console.WriteLine("✅ Ubicación del jugador eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar la ubicación del jugador: {ex.Message}");
            }
        }
        public async Task<PlayerLocation?> CreateDefaultPlayerLocation(PlayerModel player)
        {
            //Avisarle al world 

            // Avisarle al Map

            int worldId = 1;
            int mapId = 1;

            MapModel? map = await _mapService.GetMapByIdAsync(mapId);

            WorldModel? world = await _worldService.GetWorldByIdAsync(worldId);

            if (world != null && map != null)
            {
                PlayerLocation playerLocation = new PlayerLocation
                {
                    World = world,
                    Map = map,
                    PosX = 0,
                    PosY = 0,
                    Player = player
                };
                await CreatePlayerLocation(playerLocation);

                return playerLocation;
            }
            return null;
        }
        public async Task DeletePlayerLocationAsync(int playerId)
        {
            PlayerLocation? playerLocation = await GetPlayerLocationAsync(playerId);
            if (playerLocation != null)
            {
                WorldModel? world = playerLocation.World;
                MapModel? map = playerLocation.Map;

                if (world != null)
                {
                    await _worldService.RemovePlayerFromWorld(playerId, world.Id);
                }
                if (map != null)
                {
                    await _mapService.RemovePlayerFromMap(playerId, map.Id);
                }
                await _playerLocationDAO.DeletePlayerLocationAsync(playerId);

            }
        }

    }
}

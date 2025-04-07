using System;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class PlayerLocationService : BaseCachedService<PlayerLocation, int>
    {
        private readonly IPlayerLocationDAO _playerLocationDAO;
        private readonly MapService _mapService;
        private readonly WorldService _worldService;

        public PlayerLocationService(IPlayerLocationDAO playerLocationDAO, MapService mapService, WorldService worldService)
        {
            _playerLocationDAO = playerLocationDAO;
            _mapService = mapService;
            _worldService = worldService;
        }

        /// <summary>
        /// Carga la ubicación de un jugador desde la fuente original (DAO).
        /// </summary>
        protected override async Task<PlayerLocation?> LoadModelAsync(int playerId)
        {
            try
            {
                return await _playerLocationDAO.GetPlayerLocationAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar ubicación del jugador: {ex.Message}");
                return null;
            }
        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            return await GetOrLoadAsync(playerId);
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            try
            {
                await _playerLocationDAO.SavePlayerLocationAsync(location);
                Set(location.Player.Id, location); // Refrescar cache
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
                Set(playerLocation.Player.Id, playerLocation); // Agregar a la cache
                Console.WriteLine("✅ Ubicación del jugador creada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la ubicación del jugador: {ex.Message}");
            }
        }

        public async Task<PlayerLocation?> CreateDefaultPlayerLocation(PlayerModel player)
        {
            int defaultWorldId = 1;
            int defaultMapId = 1;

            MapModel? map = await _mapService.GetMapByIdAsync(defaultMapId);
            WorldModel? world = await _worldService.GetWorldByIdAsync(defaultWorldId);

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
                Invalidate(playerId); // Eliminar de la cache
                Console.WriteLine("🗑️ Ubicación del jugador eliminada correctamente.");
            }
        }
    }
}

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
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Cargando ubicación del jugador con ID: {playerId}...");
                var location = await _playerLocationDAO.GetPlayerLocationAsync(playerId);
                if (location == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"No se encontró ubicación para el jugador con ID: {playerId}.");
                }
                return location;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"❌ Error al cargar ubicación del jugador: {ex.Message}");
                return null;
            }
        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Obteniendo ubicación del jugador con ID: {playerId}...");
            return await GetOrLoadAsync(playerId);
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Guardando ubicación del jugador con ID: {location.Player.Id}...");
                await _playerLocationDAO.SavePlayerLocationAsync(location);
                Set(location.Player.Id, location); // Refrescar cache
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"✅ Ubicación del jugador con ID: {location.Player.Id} guardada correctamente.");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"❌ Error al guardar la ubicación del jugador: {ex.Message}");
            }
        }

        public async Task CreatePlayerLocation(PlayerLocation playerLocation)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Creando ubicación para el jugador con ID: {playerLocation.Player.Id}...");
                await _playerLocationDAO.CreatePlayerLocationAsync(playerLocation);
                Set(playerLocation.Player.Id, playerLocation); // Agregar a la cache
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"✅ Ubicación del jugador con ID: {playerLocation.Player.Id} creada correctamente.");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"❌ Error al crear la ubicación del jugador: {ex.Message}");
            }
        }

        public async Task<PlayerLocation?> CreateDefaultPlayerLocation(PlayerModel player)
        {
            int defaultWorldId = 1;
            int defaultMapId = 1;

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Creando ubicación predeterminada para el jugador con ID: {player.Id}...");

            MapModel? map = await _mapService.GetMapByIdAsync(defaultMapId);
            WorldModel? world = await _worldService.GetWorldByIdAsync(defaultWorldId);
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Mapa encontrado: {map.Id} World Encontrado ${world.Id}");

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

                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Ubicación predeterminada para el jugador con ID: {player.Id} creada correctamente.");
                await CreatePlayerLocation(playerLocation);
                return playerLocation;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"❌ No se pudo crear la ubicación predeterminada para el jugador con ID: {player.Id}.");
            return null;
        }

        public async Task DeletePlayerLocationAsync(int playerId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Eliminando ubicación para el jugador con ID: {playerId}...");
            
            PlayerLocation? playerLocation = await GetPlayerLocationAsync(playerId);
            if (playerLocation != null)
            {
                WorldModel? world = playerLocation.World;
                MapModel? map = playerLocation.Map;

                if (world != null)
                {
                    await _worldService.RemovePlayerFromWorld(playerId, world.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Jugador con ID: {playerId} eliminado del mundo con ID: {world.Id}.");
                }

                if (map != null)
                {
                    await _mapService.RemovePlayerFromMap(playerId, map.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"Jugador con ID: {playerId} eliminado del mapa con ID: {map.Id}.");
                }

                await _playerLocationDAO.DeletePlayerLocationAsync(playerId);
                Invalidate(playerId); // Eliminar de la cache
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"🗑️ Ubicación del jugador con ID: {playerId} eliminada correctamente.");
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerLocationService, $"❌ No se encontró ubicación para el jugador con ID: {playerId}.");
            }
        }
    }
}

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
                LoggerUtil.Log("PlayerLocationService", $"Cargando ubicación del jugador con ID: {playerId}...", ConsoleColor.Cyan);
                var location = await _playerLocationDAO.GetPlayerLocationAsync(playerId);
                if (location == null)
                {
                    LoggerUtil.Log("PlayerLocationService", $"No se encontró ubicación para el jugador con ID: {playerId}.", ConsoleColor.Yellow);
                }
                return location;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("PlayerLocationService", $"❌ Error al cargar ubicación del jugador: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            LoggerUtil.Log("PlayerLocationService", $"Obteniendo ubicación del jugador con ID: {playerId}...", ConsoleColor.Cyan);
            return await GetOrLoadAsync(playerId);
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            try
            {
                LoggerUtil.Log("PlayerLocationService", $"Guardando ubicación del jugador con ID: {location.Player.Id}...", ConsoleColor.Cyan);
                await _playerLocationDAO.SavePlayerLocationAsync(location);
                Set(location.Player.Id, location); // Refrescar cache
                LoggerUtil.Log("PlayerLocationService", $"✅ Ubicación del jugador con ID: {location.Player.Id} guardada correctamente.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("PlayerLocationService", $"❌ Error al guardar la ubicación del jugador: {ex.Message}", ConsoleColor.Red);
            }
        }

        public async Task CreatePlayerLocation(PlayerLocation playerLocation)
        {
            try
            {
                LoggerUtil.Log("PlayerLocationService", $"Creando ubicación para el jugador con ID: {playerLocation.Player.Id}...", ConsoleColor.Cyan);
                await _playerLocationDAO.CreatePlayerLocationAsync(playerLocation);
                Set(playerLocation.Player.Id, playerLocation); // Agregar a la cache
                LoggerUtil.Log("PlayerLocationService", $"✅ Ubicación del jugador con ID: {playerLocation.Player.Id} creada correctamente.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("PlayerLocationService", $"❌ Error al crear la ubicación del jugador: {ex.Message}", ConsoleColor.Red);
            }
        }

        public async Task<PlayerLocation?> CreateDefaultPlayerLocation(PlayerModel player)
        {
            int defaultWorldId = 1;
            int defaultMapId = 1;

            LoggerUtil.Log("PlayerLocationService", $"Creando ubicación predeterminada para el jugador con ID: {player.Id}...", ConsoleColor.Cyan);

            MapModel? map = await _mapService.GetMapByIdAsync(defaultMapId);
            WorldModel? world = await _worldService.GetWorldByIdAsync(defaultWorldId);
            LoggerUtil.Log("PlayerLocationService", $"Mapa encontrado: {map.Id} World Encontrado ${world.Id}", ConsoleColor.Cyan);

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

                LoggerUtil.Log("PlayerLocationService", $"Ubicación predeterminada para el jugador con ID: {player.Id} creada correctamente.", ConsoleColor.Green);
                await CreatePlayerLocation(playerLocation);
                return playerLocation;
            }

            LoggerUtil.Log("PlayerLocationService", $"❌ No se pudo crear la ubicación predeterminada para el jugador con ID: {player.Id}.", ConsoleColor.Red);
            return null;
        }

        public async Task DeletePlayerLocationAsync(int playerId)
        {
            LoggerUtil.Log("PlayerLocationService", $"Eliminando ubicación para el jugador con ID: {playerId}...", ConsoleColor.Cyan);
            
            PlayerLocation? playerLocation = await GetPlayerLocationAsync(playerId);
            if (playerLocation != null)
            {
                WorldModel? world = playerLocation.World;
                MapModel? map = playerLocation.Map;

                if (world != null)
                {
                    await _worldService.RemovePlayerFromWorld(playerId, world.Id);
                    LoggerUtil.Log("PlayerLocationService", $"Jugador con ID: {playerId} eliminado del mundo con ID: {world.Id}.", ConsoleColor.Green);
                }

                if (map != null)
                {
                    await _mapService.RemovePlayerFromMap(playerId, map.Id);
                    LoggerUtil.Log("PlayerLocationService", $"Jugador con ID: {playerId} eliminado del mapa con ID: {map.Id}.", ConsoleColor.Green);
                }

                await _playerLocationDAO.DeletePlayerLocationAsync(playerId);
                Invalidate(playerId); // Eliminar de la cache
                LoggerUtil.Log("PlayerLocationService", $"🗑️ Ubicación del jugador con ID: {playerId} eliminada correctamente.", ConsoleColor.Green);
            }
            else
            {
                LoggerUtil.Log("PlayerLocationService", $"❌ No se encontró ubicación para el jugador con ID: {playerId}.", ConsoleColor.Red);
            }
        }
    }
}

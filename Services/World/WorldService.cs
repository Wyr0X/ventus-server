using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;
using VentusServer.DataAccess.Postgres;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.Services
{
    public class WorldService
    {
        private MapService _mapService;
        private IWorldDAO _worldDAO;

        public WorldService(MapService mapService, IWorldDAO worldDAO)
        {
            LoggerUtil.Log("WorldService", "Iniciando WorldService...", ConsoleColor.Cyan);
            _mapService = mapService;
            _worldDAO = worldDAO;
            createDefaultWorld();
        }

        private async void createDefaultWorld()
        {
            LoggerUtil.Log("WorldService", "Verificando existencia del mundo predeterminado...", ConsoleColor.Yellow);
            WorldModel? existDefaultWorld = await this.GetWorldByIdAsync(1);
            MapModel? existDefaultMap = await _mapService.GetMapByIdAsync(1);
            
            if (existDefaultWorld == null)
            {
                LoggerUtil.Log("WorldService", "El mundo predeterminado no existe, creando uno nuevo...", ConsoleColor.Green);
                WorldModel? defaultWorld = await CreateWorldAsync("Nuevo Mundo", "Este es un mundo de ejemplo con parámetros predeterminados.", 10, 100, 1);
                
                if (existDefaultMap == null && defaultWorld != null)
                {
                    LoggerUtil.Log("WorldService", "El mapa predeterminado no existe, creando uno...", ConsoleColor.Green);
                    MapModel map = new MapModel
                    {
                        Id = 1,
                        Name = "Mapa Predeterminado",
                        MinLevel = 1,
                        MaxPlayers = 10,
                        WorldModel = defaultWorld,
                        WorldId = defaultWorld.Id
                    };
                    
                    MapModel? defaultMap = await _mapService.CreateMapAsync(map);
                    LoggerUtil.Log("WorldService", $"Mapa predeterminado creado exitosamente ${existDefaultMap}.", ConsoleColor.Green);
                }
                else
                {
           
                    LoggerUtil.Log("WorldService", $"El mapa predeterminado ya existe.  ${existDefaultMap} - ${defaultWorld}", ConsoleColor.Yellow);
                }
            }
            else
            {
                LoggerUtil.Log("WorldService", "El mundo predeterminado ya existe.", ConsoleColor.Yellow);
            }
        }

        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            try
            {
                LoggerUtil.Log("WorldService", "Creando un nuevo mundo...", ConsoleColor.Green);
                return await _worldDAO.CreateWorldAsync(name, description, maxMaps, maxPlayers, levelRequirements);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al crear el mundo: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            try
            {
                LoggerUtil.Log("WorldService", $"Obteniendo el mundo con ID {worldId}...", ConsoleColor.Yellow);
                return await _worldDAO.GetWorldByIdAsync(worldId);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al obtener el mundo con ID {worldId}: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            try
            {
                LoggerUtil.Log("WorldService", "Obteniendo todos los mundos...", ConsoleColor.Yellow);
                return await _worldDAO.GetAllWorldsAsync();
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al obtener todos los mundos: {ex.Message}", ConsoleColor.Red);
                return new List<WorldModel>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            try
            {
                LoggerUtil.Log("WorldService", $"Guardando mundo con ID {world.Id}...", ConsoleColor.Yellow);
                await _worldDAO.SaveWorldAsync(world);
                LoggerUtil.Log("WorldService", $"Mundo con ID {world.Id} guardado correctamente.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al guardar el mundo con ID {world.Id}: {ex.Message}", ConsoleColor.Red);
            }
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            try
            {
                LoggerUtil.Log("WorldService", $"Eliminando mundo con ID {worldId}...", ConsoleColor.Yellow);
                await _worldDAO.DeleteWorldAsync(worldId);
                LoggerUtil.Log("WorldService", $"Mundo con ID {worldId} eliminado correctamente.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al eliminar el mundo con ID {worldId}: {ex.Message}", ConsoleColor.Red);
            }
        }

        public async Task RemovePlayerFromWorld(int playerId, int worldId)
        {
            try
            {
                LoggerUtil.Log("WorldService", $"Removiendo jugador con ID {playerId} del mundo con ID {worldId}...", ConsoleColor.Yellow);
                WorldModel? world = await GetWorldByIdAsync(worldId);
                if (world != null)
                {
                    world.RemovePlayer(playerId);
                    await _worldDAO.SaveWorldAsync(world);
                    LoggerUtil.Log("WorldService", $"Jugador con ID {playerId} removido del mundo con ID {worldId}.", ConsoleColor.Green);
                }
                else
                {
                    LoggerUtil.Log("WorldService", $"No se encontró el mundo con ID {worldId}.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("WorldService", $"Error al remover jugador con ID {playerId} del mundo con ID {worldId}: {ex.Message}", ConsoleColor.Red);
            }
        }
    }
}

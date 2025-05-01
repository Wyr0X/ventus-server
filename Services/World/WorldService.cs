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
            Log.Log(Log.LogTag.WorldService, "Iniciando WorldService...");
            _mapService = mapService;
            _worldDAO = worldDAO;
            createDefaultWorld();
        }

        private async void createDefaultWorld()
        {
            Log.Log(Log.LogTag.WorldService, "Verificando existencia del mundo predeterminado...");
            WorldModel? existDefaultWorld = await this.GetWorldByIdAsync(1);
            MapModel? existDefaultMap = await _mapService.GetMapByIdAsync(1);
            
            if (existDefaultWorld == null)
            {
                Log.Log(Log.LogTag.WorldService, "El mundo predeterminado no existe, creando uno nuevo...");
                WorldModel? defaultWorld = await CreateWorldAsync("Nuevo Mundo", "Este es un mundo de ejemplo con parámetros predeterminados.", 10, 100, 1);
                
                if (existDefaultMap == null && defaultWorld != null)
                {
                    Log.Log(Log.LogTag.WorldService, "El mapa predeterminado no existe, creando uno...");
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
                    Log.Log(Log.LogTag.WorldService, $"Mapa predeterminado creado exitosamente ${existDefaultMap}.");
                }
                else
                {
           
                    Log.Log(Log.LogTag.WorldService, $"El mapa predeterminado ya existe.  ${existDefaultMap} - ${defaultWorld}");
                }
            }
            else
            {
                Log.Log(Log.LogTag.WorldService, "El mundo predeterminado ya existe.");
            }
        }

        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, "Creando un nuevo mundo...");
                return await _worldDAO.CreateWorldAsync(name, description, maxMaps, maxPlayers, levelRequirements);
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al crear el mundo: {ex.Message}");
                return null;
            }
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, $"Obteniendo el mundo con ID {worldId}...");
                return await _worldDAO.GetWorldByIdAsync(worldId);
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al obtener el mundo con ID {worldId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, "Obteniendo todos los mundos...");
                return await _worldDAO.GetAllWorldsAsync();
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al obtener todos los mundos: {ex.Message}");
                return new List<WorldModel>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, $"Guardando mundo con ID {world.Id}...");
                await _worldDAO.SaveWorldAsync(world);
                Log.Log(Log.LogTag.WorldService, $"Mundo con ID {world.Id} guardado correctamente.");
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al guardar el mundo con ID {world.Id}: {ex.Message}");
            }
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, $"Eliminando mundo con ID {worldId}...");
                await _worldDAO.DeleteWorldAsync(worldId);
                Log.Log(Log.LogTag.WorldService, $"Mundo con ID {worldId} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al eliminar el mundo con ID {worldId}: {ex.Message}");
            }
        }

        public async Task RemovePlayerFromWorld(int playerId, int worldId)
        {
            try
            {
                Log.Log(Log.LogTag.WorldService, $"Removiendo jugador con ID {playerId} del mundo con ID {worldId}...");
                WorldModel? world = await GetWorldByIdAsync(worldId);
                if (world != null)
                {
                    world.RemovePlayer(playerId);
                    await _worldDAO.SaveWorldAsync(world);
                    Log.Log(Log.LogTag.WorldService, $"Jugador con ID {playerId} removido del mundo con ID {worldId}.");
                }
                else
                {
                    Log.Log(Log.LogTag.WorldService, $"No se encontró el mundo con ID {worldId}.");
                }
            }
            catch (Exception ex)
            {
                Log.Log(Log.LogTag.WorldService, $"Error al remover jugador con ID {playerId} del mundo con ID {worldId}: {ex.Message}");
            }
        }
    }
}

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
            LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "Iniciando WorldService...");
            _mapService = mapService;
            _worldDAO = worldDAO;
            createDefaultWorld();
        }

        private async void createDefaultWorld()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "Verificando existencia del mundo predeterminado...");
            WorldModel? existDefaultWorld = await this.GetWorldByIdAsync(1).ConfigureAwait(false);
            MapModel? existDefaultMap = await _mapService.GetMapByIdAsync(1).ConfigureAwait(false);

            if (existDefaultWorld == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "El mundo predeterminado no existe, creando uno nuevo...");
                WorldModel? defaultWorld = await CreateWorldAsync("Nuevo Mundo", "Este es un mundo de ejemplo con parámetros predeterminados.", 10, 100, 1);

                if (existDefaultMap == null && defaultWorld != null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "El mapa predeterminado no existe, creando uno...");
                    MapModel map = new MapModel
                    {
                        Id = 1,
                        Name = "Mapa Predeterminado",
                        MinLevel = 1,
                        MaxPlayers = 10,
                        WorldId = defaultWorld.Id
                    };

                    MapModel? defaultMap = await _mapService.CreateMapAsync(map).ConfigureAwait(false);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Mapa predeterminado creado exitosamente ${existDefaultMap}.");
                }
                else
                {

                    LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"El mapa predeterminado ya existe.  ${existDefaultMap} - ${defaultWorld}");
                }
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "El mundo predeterminado ya existe.");
            }
        }

        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "Creando un nuevo mundo...");
                return await _worldDAO.CreateWorldAsync(name, description, maxMaps, maxPlayers, levelRequirements).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al crear el mundo: {ex.Message}");
                return null;
            }
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Obteniendo el mundo con ID {worldId}...");
                List<MapModel> mapsOfWorld = (await _mapService.GetMapsByWorldIdAsync(worldId).ConfigureAwait(false)).ToList();

                WorldModel? worldModel = await _worldDAO.GetWorldByIdAsync(worldId).ConfigureAwait(false);
                if (worldModel == null) return null;
                worldModel.AddMaps(mapsOfWorld);

                return worldModel;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al obtener el mundo con ID {worldId}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, "Obteniendo todos los mundos...");

                // Obtener todos los mundos
                List<WorldModel> worlds = await _worldDAO.GetAllWorldsAsync().ConfigureAwait(false);

                // Iterar sobre cada mundo
                foreach (var world in worlds)
                {
                    // Obtener los mapas asociados a cada mundo
                    List<MapModel> mapsOfWorld = (await _mapService.GetMapsByWorldIdAsync(world.Id).ConfigureAwait(false)).ToList();
                    world.AddMaps(mapsOfWorld); // Agregar los mapas al mundo

                }

                return worlds;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al obtener todos los mundos: {ex.Message}");
                return new List<WorldModel>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Guardando mundo con ID {world.Id}...");
                await _worldDAO.SaveWorldAsync(world).ConfigureAwait(false);
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Mundo con ID {world.Id} guardado correctamente.");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al guardar el mundo con ID {world.Id}: {ex.Message}");
            }
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Eliminando mundo con ID {worldId}...");
                await _worldDAO.DeleteWorldAsync(worldId).ConfigureAwait(false);
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Mundo con ID {worldId} eliminado correctamente.");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al eliminar el mundo con ID {worldId}: {ex.Message}");
            }
        }

        public async Task RemovePlayerFromWorld(int playerId, int worldId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Removiendo jugador con ID {playerId} del mundo con ID {worldId}...");
                WorldModel? world = await GetWorldByIdAsync(worldId).ConfigureAwait(false);
                if (world != null)
                {
                    world.RemovePlayer(playerId);
                    await _worldDAO.SaveWorldAsync(world).ConfigureAwait(false);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Jugador con ID {playerId} removido del mundo con ID {worldId}.");
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"No se encontró el mundo con ID {worldId}.");
                }
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldService, $"Error al remover jugador con ID {playerId} del mundo con ID {worldId}: {ex.Message}");
            }
        }
    }
}

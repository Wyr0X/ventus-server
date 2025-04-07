using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.Services
{
    public class MapService : BaseCachedService<MapModel, int>
    {
        private readonly IMapDAO _mapDAO;

        public MapService(IMapDAO mapDAO)
        {
            _mapDAO = mapDAO;
        }

        // =============================
        // CRUD BÁSICO
        // =============================

        public async Task<MapModel?> GetMapByIdAsync(int mapId)
        {
            try
            {
                // Primero intenta obtener el mapa de la caché
                LoggerUtil.Log("MapService", $"Intentando obtener el mapa con ID {mapId} desde la caché...", ConsoleColor.Cyan);
                return await GetOrLoadAsync(mapId);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al obtener el mapa con ID {mapId}: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            try
            {
                LoggerUtil.Log("MapService", "Obteniendo todos los mapas desde la base de datos...", ConsoleColor.Cyan);
                return await _mapDAO.GetAllMapsAsync();
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al obtener todos los mapas: {ex.Message}", ConsoleColor.Red);
                return new List<MapModel>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task<MapModel?> CreateMapAsync(MapModel map)
        {
            try
            {
                // Crear el mapa en la base de datos
                LoggerUtil.Log("MapService", $"Creando el mapa con ID {map.Id} en la base de datos...", ConsoleColor.Cyan);
                
                await _mapDAO.CreateMapAsync(map);

                // Después de crearlo, lo agregamos a la caché
                LoggerUtil.Log("MapService", $"Agregando el mapa con ID {map.Id} a la caché...", ConsoleColor.Cyan);
                Set(map.Id, map);
                LoggerUtil.Log("SUCCESS", $"Mapa con ID {map.Id} creado correctamente.", ConsoleColor.Green);
                return map;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al crear el mapa: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task SaveMapAsync(MapModel map)
        {
            try
            {
                // Actualizar el mapa en la base de datos
                LoggerUtil.Log("MapService", $"Actualizando el mapa con ID {map.Id} en la base de datos...", ConsoleColor.Cyan);
                await _mapDAO.UpdateMapAsync(map);

                // Actualizar la caché después de la actualización
                LoggerUtil.Log("MapService", $"Actualizando la caché con el mapa ID {map.Id}...", ConsoleColor.Cyan);
                Set(map.Id, map);
                LoggerUtil.Log("SUCCESS", $"Mapa con ID {map.Id} actualizado correctamente.", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al actualizar el mapa con ID {map.Id}: {ex.Message}", ConsoleColor.Red);
            }
        }

        public async Task DeleteMapAsync(int mapId)
        {
            try
            {
                LoggerUtil.Log("MapService", $"Eliminando el mapa con ID {mapId} de la base de datos...", ConsoleColor.Cyan);
                var deleted = await _mapDAO.DeleteMapAsync(mapId);
                if (deleted)
                {
                    // Invalidar la caché después de eliminar el mapa
                    LoggerUtil.Log("MapService", $"Invalidando la caché para el mapa con ID {mapId}...", ConsoleColor.Cyan);
                    Invalidate(mapId);
                    LoggerUtil.Log("SUCCESS", $"Mapa con ID {mapId} eliminado correctamente.", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al eliminar el mapa con ID {mapId}: {ex.Message}", ConsoleColor.Red);
            }
        }

        // =============================
        // REMOVER JUGADOR DEL MAPA
        // =============================

        public async Task RemovePlayerFromMap(int playerId, int mapId)
        {
            try
            {
                var map = await GetMapByIdAsync(mapId);
                if (map != null)
                {
                    LoggerUtil.Log("MapService", $"Removiendo al jugador con ID {playerId} del mapa con ID {mapId}...", ConsoleColor.Cyan);
                    map.RemovePlayer(playerId);
                    await _mapDAO.UpdateMapAsync(map); // Actualizar mapa después de modificar
                    Set(map.Id, map); // Actualizar caché
                    LoggerUtil.Log("SUCCESS", $"Jugador con ID {playerId} removido del mapa con ID {mapId}.", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al remover al jugador con ID {playerId} del mapa con ID {mapId}: {ex.Message}", ConsoleColor.Red);
            }
        }

        // =============================
        // CACHE
        // =============================

        protected override async Task<MapModel?> LoadModelAsync(int mapId)
        {
            try
            {
                LoggerUtil.Log("MapService", $"Cargando el mapa con ID {mapId} desde la base de datos...", ConsoleColor.Cyan);
                return await _mapDAO.GetMapByIdAsync(mapId);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("MapService", $"Error al cargar el mapa con ID {mapId}: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }
    }
}

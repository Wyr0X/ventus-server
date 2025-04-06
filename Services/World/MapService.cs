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
            // Primero intenta obtener el mapa de la caché
            return await GetOrLoadAsync(mapId);
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            try
            {
                return await _mapDAO.GetAllMapsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener todos los mapas: {ex.Message}");
                return new List<MapModel>(); // Retorna una lista vacía en caso de error
            }
        }

        public async Task CreateMapAsync(MapModel map)
        {
            try
            {
                // Crear el mapa en la base de datos
                await _mapDAO.CreateMapAsync(map);
                // Después de crearlo, lo agregamos a la caché
                Set(map.Id, map);
                Console.WriteLine("✅ Mapa creado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear el mapa: {ex.Message}");
            }
        }

        public async Task SaveMapAsync(MapModel map)
        {
            try
            {
                // Actualizar el mapa en la base de datos
                await _mapDAO.UpdateMapAsync(map);
                // Actualizar la caché después de la actualización
                Set(map.Id, map);
                Console.WriteLine("✅ Mapa actualizado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al actualizar el mapa: {ex.Message}");
            }
        }

        public async Task DeleteMapAsync(int mapId)
        {
            try
            {
                var deleted = await _mapDAO.DeleteMapAsync(mapId);
                if (deleted)
                {
                    // Invalidar la caché después de eliminar el mapa
                    Invalidate(mapId);
                    Console.WriteLine("✅ Mapa eliminado correctamente.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar el mapa: {ex.Message}");
            }
        }

        // =============================
        // REMOVER JUGADOR DEL MAPA
        // =============================

        public async Task RemovePlayerFromMap(int playerId, int mapId)
        {
            var map = await GetMapByIdAsync(mapId);
            if (map != null)
            {
                map.RemovePlayer(playerId);
                await _mapDAO.UpdateMapAsync(map); // Actualizar mapa después de modificar
                Set(map.Id, map); // Actualizar caché
            }
        }

        // =============================
        // CACHE
        // =============================

        protected override async Task<MapModel?> LoadModelAsync(int mapId)
        {
            try
            {
                return await _mapDAO.GetMapByIdAsync(mapId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar el mapa desde LoadModelAsync: {ex.Message}");
                return null;
            }
        }
    }
}

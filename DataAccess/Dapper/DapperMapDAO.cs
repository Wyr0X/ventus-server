using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Game.Models;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.DataAccess.Queries;
using static LoggerUtil;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperMapDAO : BaseDAO, IMapDAO
    {
        private readonly IWorldDAO _worldDAO;

        public DapperMapDAO(IDbConnectionFactory connectionFactory, IWorldDAO worldDAO) 
            : base(connectionFactory)
        {
            _worldDAO = worldDAO;
        }

        public async Task InitializeMapsAsync()
        {
            Log("MapDAO", "Inicializando tabla 'maps'...", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(MapQueries.CreateTableQuery);

            Log("MapDAO", "Tabla 'maps' inicializada correctamente", ConsoleColor.Green);
        }

        public async Task<MapModel?> GetMapByIdAsync(int id)
        {
            Log("MapDAO", $"Buscando mapa con ID: {id}", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            var mapData = await connection.QuerySingleOrDefaultAsync(MapQueries.SelectById, new { Id = id });

            if (mapData == null)
            {
                Log("MapDAO", $"Mapa con ID {id} no encontrado", ConsoleColor.Yellow);
                return null;
            }

            var map = MapMapper.Map(mapData);
            map.WorldModel = await _worldDAO.GetWorldByIdAsync(map.WorldId);

            Log("MapDAO", $"Mapa encontrado: {map.Name}", ConsoleColor.Green);
            return map;
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            Log("MapDAO", "Obteniendo todos los mapas", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync(MapQueries.SelectAll);

            var maps = new List<MapModel>();

            foreach (var row in results)
            {
                var map = MapMapper.Map(row);
                map.WorldModel = await _worldDAO.GetWorldByIdAsync(map.WorldId);
                maps.Add(map);
            }

            Log("MapDAO", $"Total mapas encontrados: {maps.Count}", ConsoleColor.Green);
            return maps;
        }

        public async Task<MapModel> CreateMapAsync(MapModel map)
        {
            Log("MapDAO", $"Creando mapa: {map.Name}", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(MapQueries.Insert, new
            {
                map.Name,
                map.MinLevel,
                map.MaxPlayers,
                map.WorldId
            });

            map.Id = id;

            Log("MapDAO", $"Mapa creado con ID: {id}", ConsoleColor.Green);
            return map;
        }

        public async Task UpdateMapAsync(MapModel map)
        {
            Log("MapDAO", $"Actualizando mapa: {map.Name} (ID: {map.Id})", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(MapQueries.Update, new
            {
                map.Id,
                map.Name,
                map.MinLevel,
                map.MaxPlayers,
                map.WorldId
            });

            Log("MapDAO", "Mapa actualizado correctamente", ConsoleColor.Green);
        }

        public async Task<bool> DeleteMapAsync(int id)
        {
            Log("MapDAO", $"Eliminando mapa con ID: {id}", ConsoleColor.Cyan);

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(MapQueries.Delete, new { Id = id });

            Log("MapDAO", rowsAffected > 0 ? "Mapa eliminado correctamente" : "No se encontró el mapa",
                rowsAffected > 0 ? ConsoleColor.Green : ConsoleColor.Yellow);

            return rowsAffected > 0;
        }
    }
}

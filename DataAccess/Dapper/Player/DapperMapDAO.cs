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
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, "Inicializando tabla 'maps'...");

            using var connection = GetConnection();
            await connection.ExecuteAsync(MapQueries.CreateTableQuery);

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, "Tabla 'maps' inicializada correctamente");
        }

        public async Task<MapModel?> GetMapByIdAsync(int id)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Buscando mapa con ID: {id}");

            using var connection = GetConnection();
            var row = await connection.QuerySingleOrDefaultAsync(MapQueries.SelectById, new { Id = id });

            if (row == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Mapa con ID {id} no encontrado");
                return null;
            }

            var map = MapMapper.Map(row);
            map.WorldModel = await _worldDAO.GetWorldByIdAsync(map.WorldId);

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Mapa encontrado: {map.Name}");
            return map;
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, "Obteniendo todos los mapas");

            using var connection = GetConnection();
            var rows = await connection.QueryAsync(MapQueries.SelectAll);

            var maps = MapMapper.MapRowsToMaps(rows);
            foreach (var map in maps)
            {
                map.WorldModel = await _worldDAO.GetWorldByIdAsync(map.WorldId);
            }

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Total mapas encontrados: {maps.Count}");
            return maps;
        }

        public async Task<MapModel?> CreateMapAsync(MapModel map)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Creando mapa: {map.Name}");

            using var connection = GetConnection();
            var id = await connection.ExecuteScalarAsync<int>(MapQueries.Insert, MapMapper.ToEntity(map));

            map.Id = id;

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Mapa creado con ID: {id}");
            return map;
        }

        public async Task UpdateMapAsync(MapModel map)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Actualizando mapa: {map.Name} (ID: {map.Id})");

            using var connection = GetConnection();
            await connection.ExecuteAsync(MapQueries.Update, MapMapper.ToEntity(map));

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, "Mapa actualizado correctamente");
        }

        public async Task<bool> DeleteMapAsync(int id)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Eliminando mapa con ID: {id}");

            using var connection = GetConnection();
            var rowsAffected = await connection.ExecuteAsync(MapQueries.Delete, new { Id = id });

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, rowsAffected > 0 ? "Mapa eliminado correctamente" : "No se encontró el mapa");

            return rowsAffected > 0;
        }
        public async Task<IEnumerable<MapModel>> GetMapsByWorldIdAsync(int worldId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Buscando mapas para WorldId: {worldId}");

            using var connection = GetConnection();
            var rows = await connection.QueryAsync(MapQueries.SelectByWorldId, new { WorldId = worldId });

            var maps = MapMapper.MapRowsToMaps(rows);
            var world = await _worldDAO.GetWorldByIdAsync(worldId);

            foreach (var map in maps)
            {
                map.WorldModel = world;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.DapperMapDAO, $"Total mapas encontrados para WorldId {worldId}: {maps.Count}");
            return maps;
        }
    }
}

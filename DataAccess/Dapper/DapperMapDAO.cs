using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Game.Models;
using VentusServer.DataAccess.Interfaces;
using static LoggerUtil;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperMapDAO : IMapDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IWorldDAO _worldDAO;

        public DapperMapDAO(IDbConnectionFactory connectionFactory, IWorldDAO worldDAO)
        {
            _connectionFactory = connectionFactory;
            _worldDAO = worldDAO;
        }

        public async Task<MapModel?> GetMapByIdAsync(int id)
        {
            Log("MapDAO", $"Buscando mapa con ID: {id}", ConsoleColor.Cyan);

            const string query = @"
                SELECT id, name, min_level, max_players, world_id
                FROM maps
                WHERE id = @Id
                LIMIT 1;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var mapData = await connection.QuerySingleOrDefaultAsync(query, new { Id = id });

            if (mapData == null)
            {
                Log("MapDAO", $"Mapa con ID {id} no encontrado", ConsoleColor.Yellow);
                return null;
            }

            var world = await _worldDAO.GetWorldByIdAsync((int)mapData.world_id);

            var map = new MapModel
            {
                Id = mapData.id,
                Name = mapData.name,
                MinLevel = mapData.min_level,
                MaxPlayers = mapData.max_players,
                WorldId = mapData.world_id,
                WorldModel = world
            };

            Log("MapDAO", $"Mapa encontrado: {map.Name}", ConsoleColor.Green);
            return map;
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            Log("MapDAO", "Obteniendo todos los mapas", ConsoleColor.Cyan);

            const string query = @"
                SELECT id, name, min_level, max_players, world_id
                FROM maps;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync(query);

            var maps = new List<MapModel>();

            foreach (var row in results)
            {
                var world = await _worldDAO.GetWorldByIdAsync((int)row.world_id);

                maps.Add(new MapModel
                {
                    Id = row.id,
                    Name = row.name,
                    MinLevel = row.min_level,
                    MaxPlayers = row.max_players,
                    WorldId = row.world_id,
                    WorldModel = world
                });
            }

            Log("MapDAO", $"Total mapas encontrados: {maps.Count}", ConsoleColor.Green);
            return maps;
        }

        public async Task<MapModel> CreateMapAsync(MapModel map)
        {
            Log("MapDAO", $"Creando mapa: {map.Name}", ConsoleColor.Cyan);

            const string query = @"
                INSERT INTO maps (name, min_level, max_players, world_id)
                VALUES (@Name, @MinLevel, @MaxPlayers, @WorldId)
                RETURNING id;
            ";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(query, new
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

            const string query = @"
                UPDATE maps
                SET name = @Name,
                    min_level = @MinLevel,
                    max_players = @MaxPlayers,
                    world_id = @WorldId
                WHERE id = @Id;
            ";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(query, new
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

            const string query = "DELETE FROM maps WHERE id = @Id;";

            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

            Log("MapDAO", rowsAffected > 0 ? "Mapa eliminado correctamente" : "No se encontró el mapa",
                rowsAffected > 0 ? ConsoleColor.Green : ConsoleColor.Yellow);

            return rowsAffected > 0;
        }
    }
}

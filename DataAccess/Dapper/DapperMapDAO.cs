using System.Data;
using Dapper;
using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperMapDAO : IMapDAO
    {
        private readonly IDbConnection _connection;
        private readonly IWorldDAO _worldDAO;

        public DapperMapDAO(IDbConnection connection, IWorldDAO worldDAO)
        {
            _connection = connection;
            _worldDAO = worldDAO;
        }

        public async Task<MapModel?> GetMapByIdAsync(int id)
        {
            const string query = @"
                SELECT id, name, min_level, max_players, world_id
                FROM maps
                WHERE id = @Id
                LIMIT 1;
            ";

            var mapData = await _connection.QuerySingleOrDefaultAsync(query, new { Id = id });

            if (mapData == null) return null;

            var world = await _worldDAO.GetWorldByIdAsync((int)mapData.world_id);

            return new MapModel
            {
                Id = mapData.id,
                Name = mapData.name,
                MinLevel = mapData.min_level,
                MaxPlayers = mapData.max_players,
                WorldId = mapData.world_id,
                WorldModel = world
            };
        }

        public async Task<IEnumerable<MapModel>> GetAllMapsAsync()
        {
            const string query = @"
                SELECT id, name, min_level, max_players, world_id
                FROM maps;
            ";

            var results = await _connection.QueryAsync(query);

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

            return maps;
        }

        public async Task CreateMapAsync(MapModel map)
        {
            const string query = @"
                INSERT INTO maps (name, min_level, max_players, world_id)
                VALUES (@Name, @MinLevel, @MaxPlayers, @WorldId)
                RETURNING id;
            ";

            var id = await _connection.ExecuteScalarAsync<int>(query, new
            {
                map.Name,
                map.MinLevel,
                map.MaxPlayers,
                map.WorldId
            });

            map.Id = id;
        }

        public async Task UpdateMapAsync(MapModel map)
        {
            const string query = @"
                UPDATE maps
                SET name = @Name,
                    min_level = @MinLevel,
                    max_players = @MaxPlayers,
                    world_id = @WorldId
                WHERE id = @Id;
            ";

            await _connection.ExecuteAsync(query, new
            {
                map.Id,
                map.Name,
                map.MinLevel,
                map.MaxPlayers,
                map.WorldId
            });
        }

        public async Task<bool> DeleteMapAsync(int id)
        {
            const string query = "DELETE FROM maps WHERE id = @Id;";
            
            // Ejecutamos la eliminación y comprobamos si se eliminó alguna fila
            var rowsAffected = await _connection.ExecuteAsync(query, new { Id = id });

            // Si rowsAffected es mayor que 0, significa que se eliminó el mapa correctamente
            return rowsAffected > 0;
        }
    }
}

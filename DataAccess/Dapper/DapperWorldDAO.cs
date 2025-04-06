using System.Data;
using Dapper;
using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperWorldDAO : IWorldDAO
    {
        private readonly IDbConnection _connection;
        private readonly IMapDAO _mapDAO;

        public DapperWorldDAO(IDbConnection connection, IMapDAO mapDAO)
        {
            _connection = connection;
            _mapDAO = mapDAO;
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int id)
        {
            const string query = @"
                SELECT id, name, description, max_maps, max_players, level_requirements
                FROM worlds
                WHERE id = @Id
                LIMIT 1;
            ";

            var worldData = await _connection.QuerySingleOrDefaultAsync(query, new { Id = id });

            if (worldData == null) return null;

            // Obtener los mapas asociados a este mundo
            var maps = await _mapDAO.GetAllMapsAsync();

            return new WorldModel
            {
                Id = worldData.id,
                Name = worldData.name,
                Description = worldData.description,
                MaxMaps = worldData.max_maps,
                MaxPlayers = worldData.max_players,
                LevelRequirements = worldData.level_requirements,
                Maps = maps.Where(map => map.WorldId == worldData.id).ToList()
            };
        }

        public async Task<IEnumerable<WorldModel>> GetAllWorldsAsync()
        {
            const string query = @"
                SELECT id, name, description, max_maps, max_players, level_requirements
                FROM worlds;
            ";

            var results = await _connection.QueryAsync(query);

            var worlds = new List<WorldModel>();

            foreach (var row in results)
            {
                // Obtener los mapas para cada mundo
                var maps = await _mapDAO.GetAllMapsAsync();

                worlds.Add(new WorldModel
                {
                    Id = row.id,
                    Name = row.name,
                    Description = row.description,
                    MaxMaps = row.max_maps,
                    MaxPlayers = row.max_players,
                    LevelRequirements = row.level_requirements,
                    Maps = maps.Where(map => map.WorldId == row.id).ToList()
                });
            }

            return worlds;
        }

        public async Task CreateWorldAsync(WorldModel world)
        {
            const string query = @"
                INSERT INTO worlds (name, description, max_maps, max_players, level_requirements)
                VALUES (@Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
                RETURNING id;
            ";

            var id = await _connection.ExecuteScalarAsync<int>(query, new
            {
                world.Name,
                world.Description,
                world.MaxMaps,
                world.MaxPlayers,
                world.LevelRequirements
            });

            world.Id = id;
        }

        public async Task UpdateWorldAsync(WorldModel world)
        {
            const string query = @"
                UPDATE worlds
                SET name = @Name,
                    description = @Description,
                    max_maps = @MaxMaps,
                    max_players = @MaxPlayers,
                    level_requirements = @LevelRequirements
                WHERE id = @Id;
            ";

            await _connection.ExecuteAsync(query, new
            {
                world.Id,
                world.Name,
                world.Description,
                world.MaxMaps,
                world.MaxPlayers,
                world.LevelRequirements
            });
        }

        public async Task DeleteWorldAsync(int id)
        {
            const string query = "DELETE FROM worlds WHERE id = @Id;";
            await _connection.ExecuteAsync(query, new { Id = id });
        }
    }
}

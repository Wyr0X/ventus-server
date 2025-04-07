using Dapper;
using Game.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperWorldDAO : IWorldDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DapperWorldDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            const string query = @"
                INSERT INTO worlds (name, description, max_maps, max_players, level_requirements)
                VALUES (@Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
                RETURNING id;";

            using var connection = _connectionFactory.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int?>(query, new
            {
                Name = name,
                Description = description,
                MaxMaps = maxMaps,
                MaxPlayers = maxPlayers,
                LevelRequirements = levelRequirements
            });

            if (id == null) return null;

            return new WorldModel
            {
                Id = id.Value,
                Name = name,
                Description = description,
                MaxMaps = maxMaps,
                MaxPlayers = maxPlayers,
                LevelRequirements = levelRequirements
            };
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            const string query = "SELECT * FROM worlds WHERE id = @WorldId";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<WorldModel>(query, new { WorldId = worldId });
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            const string query = "SELECT * FROM worlds";

            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<WorldModel>(query);
            return result.AsList();
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            const string query = @"
                INSERT INTO worlds (id, name, description, max_maps, max_players, level_requirements)
                VALUES (@Id, @Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
                ON CONFLICT (id) DO UPDATE SET
                    name = EXCLUDED.name,
                    description = EXCLUDED.description,
                    max_maps = EXCLUDED.max_maps,
                    max_players = EXCLUDED.max_players,
                    level_requirements = EXCLUDED.level_requirements;";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(query, world);
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            const string query = "DELETE FROM worlds WHERE id = @WorldId";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(query, new { WorldId = worldId });
        }
    }
}

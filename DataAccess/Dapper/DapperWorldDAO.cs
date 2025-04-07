using Dapper;
using Game.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.DataAccess.Queries;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperWorldDAO(IDbConnectionFactory connectionFactory) : BaseDAO(connectionFactory), IWorldDAO
    {
        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            using var connection = _connectionFactory.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync(WorldQueries.Insert, new
            {
                Name = name,
                Description = description,
                MaxMaps = maxMaps,
                MaxPlayers = maxPlayers,
                LevelRequirements = levelRequirements
            });

            return row == null ? null : WorldMapper.Map(row);
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync(WorldQueries.SelectById, new { WorldId = worldId });

            return row == null ? null : WorldMapper.Map(row);
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync(WorldQueries.SelectAll);

            return WorldMapper.MapRowsToWorlds(rows);
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(WorldQueries.Upsert, world);
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(WorldQueries.Delete, new { WorldId = worldId });
        }
        public class WorldInitializer(IDbConnectionFactory connectionFactory)
        {
            private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

            public async Task InitializeWorldTableAsync()
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(WorldQueries.CreateTableQuery);
            }
        }
    }
}

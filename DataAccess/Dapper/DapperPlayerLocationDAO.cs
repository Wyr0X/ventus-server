using Dapper;
using Game.Models;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperPlayerLocationDAO : BaseDAO, IPlayerLocationDAO
    {
        private readonly IPlayerDAO _playerDAO;
        private readonly IWorldDAO _worldDAO;
        private readonly IMapDAO _mapDAO;

        public DapperPlayerLocationDAO(
            IDbConnectionFactory connectionFactory,
            IPlayerDAO playerDAO,
            IWorldDAO worldDAO,
            IMapDAO mapDAO
        ) : base(connectionFactory)
        {
            _playerDAO = playerDAO;
            _worldDAO = worldDAO;
            _mapDAO = mapDAO;
        }

        public async Task InitializeTableAsync()
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(PlayerLocationQueries.CreateTableQuery);
        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync(PlayerLocationQueries.SelectByPlayerId, new { PlayerId = playerId });

            if (result == null) return null;

            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            var world = await _worldDAO.GetWorldByIdAsync((int)result.world_id);
            var map = await _mapDAO.GetMapByIdAsync((int)result.map_id);

            if (player == null || world == null || map == null) return null;

            return PlayerLocationMapper.ToModel(result, player, world, map);
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(PlayerLocationQueries.InsertOrUpdate, 
                PlayerLocationMapper.ToDbParameters(location));
        }

        public async Task CreatePlayerLocationAsync(PlayerLocation location)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(PlayerLocationQueries.Insert,
                PlayerLocationMapper.ToDbParameters(location));
        }

        public async Task DeletePlayerLocationAsync(int playerId)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(PlayerLocationQueries.Delete, new { PlayerId = playerId });
        }
    }
}

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

        public async Task<PlayerLocationModel?> GetPlayerLocationAsync(int playerId)
        {
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync(PlayerLocationQueries.SelectByPlayerId, new { PlayerId = playerId });

            if (result == null) return null;

            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            var world = await _worldDAO.GetWorldByIdAsync((int)result.world_id);
            var map = await _mapDAO.GetMapByIdAsync((int)result.map_id);

            if (player == null || world == null || map == null) return null;

            return PlayerLocationMapper.Map(result, player, world, map);
        }

        public async Task SavePlayerLocationAsync(PlayerLocationModel location)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(PlayerLocationQueries.InsertOrUpdate,
                PlayerLocationMapper.ToDbParameters(location));
        }

        public async Task CreatePlayerLocationAsync(PlayerLocationModel location)
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
        public async Task<List<int>> GetPlayesrIdsByWorldIdAsync(int worldId)
        {
            using var connection = GetConnection();

            // Ejecutar la consulta para obtener las ubicaciones de los jugadores en el mundo
            var playerLocations = await connection.QueryAsync<PlayerLocationModel>(
                PlayerLocationQueries.SelectPlayersByWorldId,
                new { WorldId = worldId }
            );

            // Crear una lista de PlayerLocationModel con datos adicionales
            var result = new List<int>();

            foreach (var location in playerLocations)
            {


                result.Add(location.PlayerId);
            }

            return result;
        }
        public async Task<List<int>> GetPlayesrIdsByMapIdAsync(int mapId)
        {
            using var connection = GetConnection();

            // Ejecutar la consulta para obtener las ubicaciones de los jugadores en el mundo
            var playerLocations = await connection.QueryAsync<PlayerLocationModel>(
                PlayerLocationQueries.SelectPlayersByMapId,
                new { MapId = mapId }
            );

            // Crear una lista de PlayerLocationModel con datos adicionales
            var result = new List<int>();

            foreach (var location in playerLocations)
            {


                result.Add(location.PlayerId);
            }

            return result;
        }

    }
}

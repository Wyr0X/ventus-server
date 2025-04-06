using System.Data;
using Dapper;
using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperPlayerLocationDAO : IPlayerLocationDAO
    {
        private readonly IDbConnection _connection;
        private readonly IPlayerDAO _playerDAO;
        private readonly WorldModel _worldDAO;
        private readonly MapModel _mapDAO;

        public DapperPlayerLocationDAO(IDbConnection connection, IPlayerDAO playerDAO, IWorldDAO worldDAO, IMapDAO mapDAO)
        {
            _connection = connection;
            _playerDAO = playerDAO;
            _worldDAO = worldDAO;
            _mapDAO = mapDAO;
        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            const string query = @"
                SELECT player_id, world_id, map_id, pos_x, pos_y
                FROM player_locations
                WHERE player_id = @PlayerId
                LIMIT 1;
            ";

            var result = await _connection.QuerySingleOrDefaultAsync(query, new { PlayerId = playerId });

            if (result == null) return null;

            // Cargar relaciones usando DAOs
            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            var world = await _worldDAO.GetWorldByIdAsync((int)result.world_id);
            var map = await _mapDAO.GetMapByIdAsync((int)result.map_id);

            if (player == null || world == null || map == null) return null;

            return new PlayerLocation
            {
                PosX = result.pos_x,
                PosY = result.pos_y,
                Player = player,
                World = world,
                Map = map
            };
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            const string query = @"
                INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y)
                VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY)
                ON CONFLICT (player_id) DO UPDATE SET
                    world_id = EXCLUDED.world_id,
                    map_id = EXCLUDED.map_id,
                    pos_x = EXCLUDED.pos_x,
                    pos_y = EXCLUDED.pos_y;
            ";

            await _connection.ExecuteAsync(query, new
            {
                PlayerId = location.Player.Id,
                WorldId = location.World.Id,
                MapId = location.Map.Id,
                PosX = location.PosX,
                PosY = location.PosY
            });
        }

        public async Task CreatePlayerLocationAsync(PlayerLocation location)
        {
            const string query = @"
                INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y)
                VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY);
            ";

            await _connection.ExecuteAsync(query, new
            {
                PlayerId = location.Player.Id,
                WorldId = location.World.Id,
                MapId = location.Map.Id,
                PosX = location.PosX,
                PosY = location.PosY
            });
        }

        public async Task DeletePlayerLocationAsync(int playerId)
        {
            const string query = "DELETE FROM player_locations WHERE player_id = @PlayerId";
            await _connection.ExecuteAsync(query, new { PlayerId = playerId });
        }
    }
}

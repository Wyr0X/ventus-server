using System;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;
using System.Data;

namespace VentusServer.DataAccess.Postgres
{

    public class PostgresPlayerLocationDAO : IPlayerLocationDAO
    {
        private readonly string _connectionString;
        private PostgresWorldDAO _worldDAO;
        private PostgresMapDAO _mapDAO;
        private PostgresPlayerDAO _playerDAO;



        public PostgresPlayerLocationDAO(string connectionString, PostgresWorldDAO worldDAO, PostgresMapDAO mapDAO, PostgresPlayerDAO playerDAO)
        {
            _connectionString = connectionString;
            _worldDAO = worldDAO;
            _mapDAO = mapDAO;
            _playerDAO = playerDAO;

        }

        public async Task<PlayerLocation?> GetPlayerLocationAsync(int playerId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM player_locations WHERE player_id = @PlayerId LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", playerId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int worldId = reader.GetInt32(reader.GetInt32("world_id"));
                int mapId = reader.GetInt32(reader.GetInt32("map_id"));

                World? world = await _worldDAO.GetWorldByIdAsync(worldId);
                MapModel? map = await _mapDAO.GetMapByIdAsync(mapId);
                PlayerModel? player = await _playerDAO.GetPlayerByIdAsync(playerId);
                if (player == null || map == null || world == null) return null;
                return new PlayerLocation
                {
                    PosX = reader.GetInt32(reader.GetOrdinal("pos_x")),
                    PosY = reader.GetInt32(reader.GetOrdinal("pos_y")),
                    World = world,
                    Map = map,
                    Player = player
                };
            }
            return null;
        }

        public async Task SavePlayerLocationAsync(PlayerLocation location)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y, direction)
                VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY, @Direction)
                ON CONFLICT (player_id) DO UPDATE SET
                    world_id = EXCLUDED.world_id,
                    map_id = EXCLUDED.map_id,
                    pos_x = EXCLUDED.pos_x,
                    pos_y = EXCLUDED.pos_y,
                    direction = EXCLUDED.direction;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", location.Player.Id);
            command.Parameters.AddWithValue("@WorldId", location.World.Id);
            command.Parameters.AddWithValue("@MapId", location.Map.Id);
            command.Parameters.AddWithValue("@PosX", location.PosX);
            command.Parameters.AddWithValue("@PosY", location.PosY);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeletePlayerLocationAsync(int playerId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM player_locations WHERE player_id = @PlayerId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", playerId);
            await command.ExecuteNonQueryAsync();
        }
        public async Task CreatePlayerLocationAsync(PlayerLocation location)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
        INSERT INTO player_locations (player_id, world_id, map_id, pos_x, pos_y, direction)
        VALUES (@PlayerId, @WorldId, @MapId, @PosX, @PosY, @Direction);
    ";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", location.Player.Id);
            command.Parameters.AddWithValue("@WorldId", location.World.Id);
            command.Parameters.AddWithValue("@MapId", location.Map.Id);
            command.Parameters.AddWithValue("@PosX", location.PosX);
            command.Parameters.AddWithValue("@PosY", location.PosY);
            command.Parameters.AddWithValue("@Direction", "NORTH"); // Puedes establecer la dirección como un valor predeterminado si no está en el modelo

            await command.ExecuteNonQueryAsync();
        }

    }
}

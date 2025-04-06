using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;

namespace VentusServer.DataAccess.Postgres
{

    public class PostgresMapDAO
    {
        private readonly string _connectionString;
        private PostgresWorldDAO _worldDAO;

        public PostgresMapDAO(string connectionString, PostgresWorldDAO worldDao)
        {
            _connectionString = connectionString;
            _worldDAO = worldDao;
        }
        public async Task<MapModel?> CreateMapAsync(string name, int minLevel, int maxPlayers, int worldId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            WorldModel? world = await _worldDAO.GetWorldByIdAsync(worldId);
            if (world == null) return null;
            const string query = @"
        INSERT INTO maps (name, min_level, max_players, world_id)
        VALUES (@Name, @MinLevel, @MaxPlayers, @WorldId)
        RETURNING id;"; // Usamos RETURNING para obtener el ID autogenerado.

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@MinLevel", minLevel);
            command.Parameters.AddWithValue("@MaxPlayers", maxPlayers);
            command.Parameters.AddWithValue("@WorldId", worldId);

            // Obtener el ID autogenerado del mapa
            var mapId = await command.ExecuteScalarAsync();

            // Establecer el ID en el objeto Map
            if (mapId != null)
            {
                MapModel map = new MapModel
                {
                    Id = Convert.ToInt32(mapId),
                    Name = name,
                    MinLevel = minLevel,
                    MaxPlayers = maxPlayers,
                    WorldModel = world
                };
                return map;
            }
            return null;
        }

        public async Task<MapModel?> GetMapByIdAsync(int mapId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM maps WHERE id = @MapId";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@MapId", mapId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {

                return new MapModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    MinLevel = reader.GetInt32(reader.GetOrdinal("min_level")),
                    MaxPlayers = reader.GetInt32(reader.GetOrdinal("max_players")),
                    WorldId = reader.GetInt32(reader.GetOrdinal("world_id"))
                };
            }
            return null;
        }

        public async Task<List<MapModel>> GetAllMapsAsync()
        {
            var maps = new List<MapModel>();
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM maps";
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                maps.Add(new MapModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    MinLevel = reader.GetInt32(reader.GetOrdinal("min_level")),
                    MaxPlayers = reader.GetInt32(reader.GetOrdinal("max_players")),
                    WorldId = reader.GetInt32(reader.GetOrdinal("world_id"))
                });
            }
            return maps;
        }

        public async Task SaveMapAsync(MapModel map)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO maps (id, name, min_level, max_players, world_id)
                VALUES (@Id, @Name, @MinLevel, @MaxPlayers, @WorldId)
                ON CONFLICT (id) DO UPDATE SET
                    name = EXCLUDED.name,
                    min_level = EXCLUDED.min_level,
                    max_players = EXCLUDED.max_players,
                    world_id = EXCLUDED.world_id;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", map.Id);
            command.Parameters.AddWithValue("@Name", map.Name);
            command.Parameters.AddWithValue("@MinLevel", map.MinLevel);
            command.Parameters.AddWithValue("@MaxPlayers", map.MaxPlayers);
            command.Parameters.AddWithValue("@WorldId", map.WorldId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteMapAsync(int mapId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM maps WHERE id = @MapId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@MapId", mapId);
            await command.ExecuteNonQueryAsync();
        }
    }
}

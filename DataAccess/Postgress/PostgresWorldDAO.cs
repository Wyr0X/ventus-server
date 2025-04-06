using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;

namespace VentusServer.DataAccess.Postgres
{

    public class PostgresWorldDAO
    {
        private readonly string _connectionString;

        public PostgresWorldDAO(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int MaxPlayers, int levelRequirements)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
        INSERT INTO worlds (name, description, max_maps, max_players, level_requirements)
        VALUES (@Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
        RETURNING id;"; // Usamos RETURNING para obtener el ID autogenerado.

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@MaxMaps", maxMaps);
            command.Parameters.AddWithValue("@MaxPlayers", MaxPlayers);
            command.Parameters.AddWithValue("@LevelRequirements", levelRequirements);

            // Obtener el ID autogenerado del mundo
            var worldId = await command.ExecuteScalarAsync();



            // Establecer el ID en el objeto World
            if (worldId != null)
            {
                WorldModel world = new WorldModel
                {
                    Id = Convert.ToInt32(worldId),
                    Name = name,
                    Description = description,
                    MaxMaps = maxMaps,
                    MaxPlayers = MaxPlayers,
                    LevelRequirements = levelRequirements
                };
                return world;
            }

            return null;
        }
        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM worlds WHERE id = @WorldId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@WorldId", worldId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {

                return new WorldModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    MaxMaps = reader.GetInt32(reader.GetOrdinal("max_maps")),
                    MaxPlayers = reader.GetInt32(reader.GetOrdinal("max_players")),
                    LevelRequirements = reader.GetInt32(reader.GetOrdinal("level_requirements"))
                };
            }
            return null;
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            var worlds = new List<WorldModel>();
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM worlds";
            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                worlds.Add(new WorldModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    MaxMaps = reader.GetInt32(reader.GetOrdinal("max_maps")),
                    MaxPlayers = reader.GetInt32(reader.GetOrdinal("max_players")),
                    LevelRequirements = reader.GetInt32(reader.GetOrdinal("level_requirements"))
                });
            }
            return worlds;
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO worlds (id, name, description, max_maps, max_players, level_requirements)
                VALUES (@Id, @Name, @Description, @MaxMaps, @MaxPlayers, @LevelRequirements)
                ON CONFLICT (id) DO UPDATE SET
                    name = EXCLUDED.name,
                    description = EXCLUDED.description,
                    max_maps = EXCLUDED.max_maps,
                    max_players = EXCLUDED.max_players,
                    level_requirements = EXCLUDED.level_requirements;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", world.Id);
            command.Parameters.AddWithValue("@Name", world.Name);
            command.Parameters.AddWithValue("@Description", world.Description);
            command.Parameters.AddWithValue("@MaxMaps", world.MaxMaps);
            command.Parameters.AddWithValue("@MaxPlayers", world.MaxPlayers);
            command.Parameters.AddWithValue("@LevelRequirements", world.LevelRequirements);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM worlds WHERE id = @WorldId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@WorldId", worldId);
            await command.ExecuteNonQueryAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using Npgsql;
using VentusServer.DataAccess;
using VentusServer.Models;

namespace VentusServer.DataAccess.Postgres
{
    public class PostgresPlayerDAO
    {
        private readonly string _connectionString;

        public PostgresPlayerDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM players WHERE id = @PlayerId LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", playerId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PlayerModel(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("account_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("gender")),
                    reader.GetString(reader.GetOrdinal("race")),
                    reader.GetInt32(reader.GetOrdinal("level")),
                    reader.GetString(reader.GetOrdinal("class"))
                );
            }
            return null;
        }

        public async Task SavePlayerAsync(PlayerModel player)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO players (id, account_id, name, gender, race, level, class, created_at, last_login, status)
                VALUES (@Id, @UserId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
                ON CONFLICT (id) DO UPDATE SET
                    account_id = EXCLUDED.account_id,
                    name = EXCLUDED.name,
                    gender = EXCLUDED.gender,
                    race = EXCLUDED.race,
                    level = EXCLUDED.level,
                    class = EXCLUDED.class,
                    last_login = EXCLUDED.last_login,
                    status = EXCLUDED.status;";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", player.Id);
            command.Parameters.AddWithValue("@UserId", player.AccountId);
            command.Parameters.AddWithValue("@Name", player.Name);
            command.Parameters.AddWithValue("@Gender", player.Gender);
            command.Parameters.AddWithValue("@Race", player.Race);
            command.Parameters.AddWithValue("@Level", player.Level);
            command.Parameters.AddWithValue("@Class", player.Class);
            command.Parameters.AddWithValue("@CreatedAt", player.CreatedAt);
            command.Parameters.AddWithValue("@LastLogin", player.LastLogin);
            command.Parameters.AddWithValue("@Status", player.Status);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM players WHERE id = @PlayerId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", playerId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Devuelve true si se eliminó al menos una fila, false si no se eliminó nada.
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT 1 FROM players WHERE id = @PlayerId LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerId", playerId);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }
        public async Task<PlayerModel> CreatePlayerAsync(Guid accountId, string name, string gender, string race, string playerClass)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // Crear el nuevo PlayerModel con los datos recibidos
            var newPlayer = new PlayerModel(
                id: 0,  // El ID se genera automáticamente por la base de datos
                accountId: accountId,
                name: name,
                gender: gender,
                race: race,
                level: 0,
                playerClass: playerClass
            );

            const string query = @"
        INSERT INTO players (account_id, name, gender, race, level, class, created_at, last_login, status)
        VALUES (@UserId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
        RETURNING id;";  // Devuelve el ID generado por la base de datos

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", newPlayer.AccountId);
            command.Parameters.AddWithValue("@Name", newPlayer.Name);
            command.Parameters.AddWithValue("@Gender", newPlayer.Gender);
            command.Parameters.AddWithValue("@Race", newPlayer.Race);
            command.Parameters.AddWithValue("@Level", newPlayer.Level);
            command.Parameters.AddWithValue("@Class", newPlayer.Class);
            command.Parameters.AddWithValue("@CreatedAt", newPlayer.CreatedAt);
            command.Parameters.AddWithValue("@LastLogin", newPlayer.LastLogin);
            command.Parameters.AddWithValue("@Status", newPlayer.Status);

            // Ejecutar la consulta y obtener el ID del nuevo jugador
            var result = await command.ExecuteScalarAsync();  // No usar <int> aquí
            if (result != null)
            {
                newPlayer.Id = Convert.ToInt32(result);  // Convertir el resultado a int
            }

            return newPlayer;  // Retornar el modelo con el ID asignado
        }
        public async Task<List<PlayerModel>> GetAllPlayersAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM players";  // Consulta para obtener todos los jugadores

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            var players = new List<PlayerModel>();

            while (await reader.ReadAsync())
            {
                var player = new PlayerModel(
                    reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("account_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("gender")),
                    reader.GetString(reader.GetOrdinal("race")),
                    reader.GetInt32(reader.GetOrdinal("level")),
                    reader.GetString(reader.GetOrdinal("class"))
                );
                players.Add(player);
            }

            return players;
        }
        public async Task<List<PlayerModel>> GetAllPlayersByUserIdAsync(string userId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM players WHERE account_id = @UserId";  // Consulta para obtener todos los jugadores de un usuario

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);  // Se pasa el userId como parámetro

            await using var reader = await command.ExecuteReaderAsync();

            var players = new List<PlayerModel>();

            while (await reader.ReadAsync())
            {
                var player = new PlayerModel(
                    reader.GetInt32(reader.GetOrdinal("id")),
                     reader.GetGuid(reader.GetOrdinal("account_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("gender")),
                    reader.GetString(reader.GetOrdinal("race")),
                    reader.GetInt32(reader.GetOrdinal("level")),
                    reader.GetString(reader.GetOrdinal("class"))
                );
                players.Add(player);
            }

            return players;
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountIdAsync(Guid accountId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM players WHERE account_id = @AccountId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            await using var reader = await command.ExecuteReaderAsync();
            var players = new List<PlayerModel>();

            while (await reader.ReadAsync())
            {
                players.Add(new PlayerModel(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("account_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("gender")),
                    reader.GetString(reader.GetOrdinal("race")),
                    reader.GetInt32(reader.GetOrdinal("level")),
                    reader.GetString(reader.GetOrdinal("class"))
                ));
            }

            return players;
        }
        public async Task<PlayerModel?> GetPlayerByNameAsync(string name)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM players WHERE name = @Name LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PlayerModel(
                    reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetGuid(reader.GetOrdinal("account_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("gender")),
                    reader.GetString(reader.GetOrdinal("race")),
                    reader.GetInt32(reader.GetOrdinal("level")),
                    reader.GetString(reader.GetOrdinal("class"))
                );
            }

            return null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Models;
using static LoggerUtil;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperPlayerDAO : IPlayerDAO
    {
        private readonly string _connectionString;

        public DapperPlayerDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            Log("PlayerDAO", $"Buscando jugador por ID: {playerId}", ConsoleColor.Cyan);

            const string query = "SELECT * FROM players WHERE id = @PlayerId LIMIT 1";

            using var connection = CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync(query, new { PlayerId = playerId });

            if (row == null)
            {
                Log("PlayerDAO", $"Jugador con ID {playerId} no encontrado.", ConsoleColor.Yellow);
                return null;
            }

            var player = MapRowToPlayer(row);

            Log("PlayerDAO", $"Jugador encontrado: {player.Name}", ConsoleColor.Green);
            return player;
        }

        public async Task<PlayerModel?> GetPlayerByNameAsync(string name)
        {
            Log("PlayerDAO", $"Buscando jugador por nombre: {name}", ConsoleColor.Cyan);

            const string query = "SELECT * FROM players WHERE name = @Name LIMIT 1";

            using var connection = CreateConnection();
            var row = await connection.QueryFirstOrDefaultAsync(query, new { Name = name });

            if (row == null)
            {
                Log("PlayerDAO", $"Jugador con nombre '{name}' no encontrado.", ConsoleColor.Yellow);
                return null;
            }

            var player = MapRowToPlayer(row);

            Log("PlayerDAO", $"Jugador encontrado: {player.Name}", ConsoleColor.Green);
            return player;
        }

        public async Task<List<PlayerModel>> GetAllPlayersAsync()
        {
            Log("PlayerDAO", "Obteniendo todos los jugadores", ConsoleColor.Cyan);

            const string query = "SELECT * FROM players";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync(query);

            var players = new List<PlayerModel>();
            foreach (var row in result)
                players.Add(MapRowToPlayer(row));

            Log("PlayerDAO", $"Total jugadores encontrados: {players.Count}", ConsoleColor.Green);
            return players;
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountIdAsync(Guid accountId)
        {
            Log("PlayerDAO", $"Buscando jugadores por Account ID: {accountId}", ConsoleColor.Cyan);

            const string query = "SELECT * FROM players WHERE account_id = @AccountId";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync(query, new { AccountId = accountId });

            var players = new List<PlayerModel>();
            foreach (var row in result)
                players.Add(MapRowToPlayer(row));

            Log("PlayerDAO", $"Se encontraron {players.Count} jugadores para Account ID: {accountId}", ConsoleColor.Green);
            return players;
        }

        public async Task<List<PlayerModel>> GetAllPlayersByUserIdAsync(string userId)
        {
            Log("PlayerDAO", $"Buscando jugadores por User ID: {userId}", ConsoleColor.Cyan);

            const string query = "SELECT * FROM players WHERE account_id = @UserId";

            using var connection = CreateConnection();
            var result = await connection.QueryAsync(query, new { UserId = Guid.Parse(userId) });

            var players = new List<PlayerModel>();
            foreach (var row in result)
                players.Add(MapRowToPlayer(row));

            Log("PlayerDAO", $"Jugadores encontrados para User ID {userId}: {players.Count}", ConsoleColor.Green);
            return players;
        }

        public async Task<PlayerModel> CreatePlayerAsync(Guid accountId, string name, string gender, string race, string playerClass)
        {
            Log("PlayerDAO", $"Creando nuevo jugador: {name} para la cuenta {accountId}", ConsoleColor.Cyan);

            const string query = @"
                INSERT INTO players (account_id, name, gender, race, level, class, created_at, last_login, status)
                VALUES (@AccountId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
                RETURNING id;
            ";

            var newPlayer = new PlayerModel(
                id: 0,
                accountId: accountId,
                name: name,
                gender: gender,
                race: race,
                level: 0,
                playerClass: playerClass
            )
            {
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Status = "active"
            };

            using var connection = CreateConnection();
            var newId = await connection.ExecuteScalarAsync<int>(query, newPlayer);
            newPlayer.Id = newId;

            Log("PlayerDAO", $"Jugador creado con ID: {newId}", ConsoleColor.Green);
            return newPlayer;
        }

        public async Task SavePlayerAsync(PlayerModel player)
        {
            Log("PlayerDAO", $"Guardando jugador: {player.Name} (ID: {player.Id})", ConsoleColor.Cyan);

            const string query = @"
                INSERT INTO players (id, account_id, name, gender, race, level, class, created_at, last_login, status)
                VALUES (@Id, @AccountId, @Name, @Gender, @Race, @Level, @Class, @CreatedAt, @LastLogin, @Status)
                ON CONFLICT (id) DO UPDATE SET
                    account_id = EXCLUDED.account_id,
                    name = EXCLUDED.name,
                    gender = EXCLUDED.gender,
                    race = EXCLUDED.race,
                    level = EXCLUDED.level,
                    class = EXCLUDED.class,
                    last_login = EXCLUDED.last_login,
                    status = EXCLUDED.status;
            ";

            using var connection = CreateConnection();
            await connection.ExecuteAsync(query, player);

            Log("PlayerDAO", $"Jugador guardado correctamente: {player.Name}", ConsoleColor.Green);
        }

        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            Log("PlayerDAO", $"Eliminando jugador con ID: {playerId}", ConsoleColor.Cyan);

            const string query = "DELETE FROM players WHERE id = @PlayerId";

            using var connection = CreateConnection();
            var affected = await connection.ExecuteAsync(query, new { PlayerId = playerId });

            Log("PlayerDAO", affected > 0 ? "Jugador eliminado." : "Jugador no encontrado.",
                affected > 0 ? ConsoleColor.Green : ConsoleColor.Yellow);

            return affected > 0;
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            Log("PlayerDAO", $"Verificando si existe el jugador con ID: {playerId}", ConsoleColor.Cyan);

            const string query = "SELECT EXISTS(SELECT 1 FROM players WHERE id = @PlayerId)";

            using var connection = CreateConnection();
            var exists = await connection.ExecuteScalarAsync<bool>(query, new { PlayerId = playerId });

            Log("PlayerDAO", $"Jugador {(exists ? "existe" : "no existe")}.", exists ? ConsoleColor.Green : ConsoleColor.Yellow);
            return exists;
        }

        public async Task<bool> PlayerNameExistsAsync(string name)
        {
            Log("PlayerDAO", $"Verificando si existe el nombre de jugador: {name}", ConsoleColor.Cyan);

            const string query = "SELECT EXISTS(SELECT 1 FROM players WHERE name = @Name)";

            using var connection = CreateConnection();
            var exists = await connection.ExecuteScalarAsync<bool>(query, new { Name = name });

            Log("PlayerDAO", $"Nombre {(exists ? "ya existe" : "disponible")}.", exists ? ConsoleColor.Yellow : ConsoleColor.Green);
            return exists;
        }

        private static PlayerModel MapRowToPlayer(dynamic row)
        {
            return new PlayerModel(
                id: row.id,
                accountId: row.account_id,
                name: row.name,
                gender: row.gender,
                race: row.race,
                level: row.level,
                playerClass: row.@class
            )
            {
                CreatedAt = row.created_at,
                LastLogin = row.last_login,
                Status = row.status
            };
        }
    }
}

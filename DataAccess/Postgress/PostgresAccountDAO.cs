using Npgsql;
using System;
using System.Threading.Tasks;
using VentusServer.DataAccess;

namespace VentusServer.DataAccess.Postgres
{
    public class PostgresAccountDAO
    {
        private readonly string _connectionString;

        public PostgresAccountDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static AccountModel MapAccount(NpgsqlDataReader reader)
        {
            return new AccountModel
            {
                AccountId = reader.GetGuid(reader.GetOrdinal("account_id")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                AccountName = reader.GetString(reader.GetOrdinal("account_name")),
                PasswordHash = reader.GetString(reader.GetOrdinal("password")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("is_deleted")),
                IsBanned = reader.GetBoolean(reader.GetOrdinal("is_banned")),
                Credits = reader.GetInt32(reader.GetOrdinal("credits")),
                LastIpAddress = reader.GetString(reader.GetOrdinal("last_ip")),
                LastLogin = reader.GetDateTime(reader.GetOrdinal("last_login")).ToUniversalTime(),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")).ToUniversalTime(),
                SessionId = reader.IsDBNull(reader.GetOrdinal("session_id"))
                    ? Guid.Empty
                    : reader.GetGuid(reader.GetOrdinal("session_id"))
            };
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM accounts WHERE email = @Email LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapAccount(reader) : null;
        }

        public async Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM accounts WHERE account_id = @AccountId LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapAccount(reader) : null;
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string accountName)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM accounts WHERE account_name = @AccountName LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountName", accountName);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapAccount(reader) : null;
        }

        public async Task SaveAccountAsync(AccountModel account)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
            INSERT INTO accounts (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id)
            VALUES (@AccountId, @Email, @AccountName, @Password, @IsDeleted, @IsBanned, @Credits, @LastIp, @LastLogin, @CreatedAt, @SessionId)
            ON CONFLICT (email, account_name) 
            DO UPDATE SET 
                password = COALESCE(@Password, accounts.password), 
                is_deleted = EXCLUDED.is_deleted, 
                is_banned = EXCLUDED.is_banned, 
                credits = EXCLUDED.credits, 
                last_ip = EXCLUDED.last_ip, 
                last_login = EXCLUDED.last_login,
                session_id = EXCLUDED.session_id";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", account.AccountId);
            command.Parameters.AddWithValue("@Email", account.Email);
            command.Parameters.AddWithValue("@AccountName", account.AccountName);
            command.Parameters.AddWithValue("@Password", account.PasswordHash);
            command.Parameters.AddWithValue("@IsDeleted", account.IsDeleted);
            command.Parameters.AddWithValue("@IsBanned", account.IsBanned);
            command.Parameters.AddWithValue("@Credits", account.Credits);
            command.Parameters.AddWithValue("@LastIp", (object?)account.LastIpAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastLogin", account.LastLogin);
            command.Parameters.AddWithValue("@CreatedAt", account.CreatedAt);
            command.Parameters.AddWithValue("@SessionId", account.SessionId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAccountAsync(string email)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM accounts WHERE email = @Email";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> AccountExistsAsync(string accountId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT 1 FROM accounts WHERE account_id = @AccountId LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task<bool> AccountExistsByNameAsync(string accountName)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT 1 FROM accounts WHERE account_name = @AccountName LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountName", accountName);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task CreateAccountAsync(AccountModel account)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO accounts (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id)
                VALUES (@AccountId, @Email, @AccountName, @Password, @IsDeleted, @IsBanned, @Credits, @LastIp, @LastLogin, @CreatedAt, @SessionId)";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", account.AccountId);
            command.Parameters.AddWithValue("@Email", account.Email);
            command.Parameters.AddWithValue("@AccountName", account.AccountName);
            command.Parameters.AddWithValue("@Password", account.PasswordHash);
            command.Parameters.AddWithValue("@IsDeleted", account.IsDeleted);
            command.Parameters.AddWithValue("@IsBanned", account.IsBanned);
            command.Parameters.AddWithValue("@Credits", account.Credits);
            command.Parameters.AddWithValue("@LastIp", (object?)account.LastIpAddress ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastLogin", account.LastLogin);
            command.Parameters.AddWithValue("@CreatedAt", account.CreatedAt);
            command.Parameters.AddWithValue("@SessionId", account.SessionId);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "UPDATE accounts SET password = @Password WHERE account_id = @AccountId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Password", newPassword);
            command.Parameters.AddWithValue("@AccountId", accountId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "UPDATE accounts SET account_name = @AccountName WHERE account_id = @AccountId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountName", accountName);
            command.Parameters.AddWithValue("@AccountId", accountId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "UPDATE accounts SET session_id = @SessionId WHERE account_id = @AccountId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@SessionId", sessionId);
            command.Parameters.AddWithValue("@AccountId", accountId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string query = "SELECT 1 FROM accounts WHERE LOWER(email) = LOWER(@Email) LIMIT 1";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("Email", email);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task<bool> IsNameTakenAsync(string accountName)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string query = "SELECT 1 FROM accounts WHERE LOWER(account_name) = LOWER(@AccountName) LIMIT 1";
            using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("AccountName", accountName);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }
    }
}

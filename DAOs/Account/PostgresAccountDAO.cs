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
                Name = reader.GetString(reader.GetOrdinal("name")),
                Password = reader.GetString(reader.GetOrdinal("password")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("is_deleted")),
                IsBanned = reader.GetBoolean(reader.GetOrdinal("is_banned")),
                Credits = reader.GetInt32(reader.GetOrdinal("credits")),
                LastIp = reader.GetString(reader.GetOrdinal("last_ip")),
                LastLogin = reader.GetDateTime(reader.GetOrdinal("last_login")).ToUniversalTime(),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")).ToUniversalTime()
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

        public async Task<AccountModel?> GetAccountByNameAsync(string name)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM accounts WHERE Name = @Name LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapAccount(reader) : null;
        }

        public async Task SaveAccountAsync(AccountModel account)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            const string query = @"
                INSERT INTO accounts (account_id, email, Name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at)
                VALUES (@AccountId, @Email, @Name, @Password, @IsDeleted, @IsBanned, @Credits, @LastIp, @LastLogin, @CreatedAt)
                ON CONFLICT (email, Name) 
                DO UPDATE SET 
                    password = COALESCE(@Password, accounts.password), 
                    is_deleted = EXCLUDED.is_deleted, 
                    is_banned = EXCLUDED.is_banned, 
                    credits = EXCLUDED.credits, 
                    last_ip = EXCLUDED.last_ip, 
                    last_login = EXCLUDED.last_login";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", account.AccountId);
            command.Parameters.AddWithValue("@Email", account.Email);
            command.Parameters.AddWithValue("@Name", account.Name);
            command.Parameters.AddWithValue("@Password", account.Password);
            command.Parameters.AddWithValue("@IsDeleted", account.IsDeleted);
            command.Parameters.AddWithValue("@IsBanned", account.IsBanned);
            command.Parameters.AddWithValue("@Credits", account.Credits);
            command.Parameters.AddWithValue("@LastIp", (object?)account.LastIp ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastLogin", account.LastLogin);
            command.Parameters.AddWithValue("@CreatedAt", account.CreatedAt);

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

        public async Task<bool> AccountExistsByNameAsync(string name)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT 1 FROM accounts WHERE Name = @Name LIMIT 1";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", name);

            await using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }

        public async Task CreateAccountAsync(AccountModel account)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO accounts (account_id, email, Name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at)
                VALUES (@AccountId, @Email, @Name, @Password, @IsDeleted, @IsBanned, @Credits, @LastIp, @LastLogin, @CreatedAt)";

            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", account.AccountId);
            command.Parameters.AddWithValue("@Email", account.Email);
            command.Parameters.AddWithValue("@Name", account.Name);
            command.Parameters.AddWithValue("@Password", account.Password);
            command.Parameters.AddWithValue("@IsDeleted", account.IsDeleted);
            command.Parameters.AddWithValue("@IsBanned", account.IsBanned);
            command.Parameters.AddWithValue("@Credits", account.Credits);
            command.Parameters.AddWithValue("@LastIp", (object?)account.LastIp ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastLogin", account.LastLogin);
            command.Parameters.AddWithValue("@CreatedAt", account.CreatedAt);

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

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "UPDATE accounts SET name = @Name WHERE account_id = @AccountId";
            await using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", newName);
            command.Parameters.AddWithValue("@AccountId", accountId);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}

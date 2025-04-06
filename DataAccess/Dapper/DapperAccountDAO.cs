using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using VentusServer.DataAccess;

namespace VentusServer.DataAccess.Postgres
{
    public class DapperAccountDAO : IAccountDAO
    {
        private readonly string _connectionString;

        public DapperAccountDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

        private AccountModel MapToAccountModel(dynamic row)
        {
            return new AccountModel
            {
                AccountId = row.account_id,
                Email = row.email,
                AccountName = row.account_name,
                PasswordHash = row.password,
                IsDeleted = row.is_deleted,
                IsBanned = row.is_banned,
                Credits = row.credits,
                LastIpAddress = row.last_ip,
                LastLogin = row.last_login,
                CreatedAt = row.created_at,
                SessionId = row.session_id
            };
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            const string query = "SELECT * FROM accounts WHERE email = @Email LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync(query, new { Email = email });
            return result == null ? null : MapToAccountModel(result);
        }

        public async Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId)
        {
            const string query = "SELECT * FROM accounts WHERE account_id = @AccountId LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync(query, new { AccountId = accountId });
            return result == null ? null : MapToAccountModel(result);
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string accountName)
        {
            const string query = "SELECT * FROM accounts WHERE account_name = @AccountName LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync(query, new { AccountName = accountName });
            return result == null ? null : MapToAccountModel(result);
        }

        public async Task SaveAccountAsync(AccountModel account)
        {
            const string query = @"
                INSERT INTO accounts (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id)
                VALUES (@AccountId, @Email, @AccountName, @PasswordHash, @IsDeleted, @IsBanned, @Credits, @LastIpAddress, @LastLogin, @CreatedAt, @SessionId)
                ON CONFLICT (email, account_name) DO UPDATE SET 
                    password = COALESCE(@PasswordHash, accounts.password), 
                    is_deleted = EXCLUDED.is_deleted, 
                    is_banned = EXCLUDED.is_banned, 
                    credits = EXCLUDED.credits, 
                    last_ip = EXCLUDED.last_ip, 
                    last_login = EXCLUDED.last_login,
                    session_id = EXCLUDED.session_id";

            using var conn = CreateConnection();
            await conn.ExecuteAsync(query, account);
        }

        public async Task DeleteAccountAsync(string email)
        {
            const string query = "DELETE FROM accounts WHERE email = @Email";
            using var conn = CreateConnection();
            await conn.ExecuteAsync(query, new { Email = email });
        }

        public async Task<bool> AccountExistsAsync(string accountId)
        {
            const string query = "SELECT 1 FROM accounts WHERE account_id = @AccountId LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.ExecuteScalarAsync<int?>(query, new { AccountId = Guid.Parse(accountId) });
            return result.HasValue;
        }

        public async Task<bool> AccountExistsByNameAsync(string accountName)
        {
            const string query = "SELECT 1 FROM accounts WHERE account_name = @AccountName LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.ExecuteScalarAsync<int?>(query, new { AccountName = accountName });
            return result.HasValue;
        }

        public async Task CreateAccountAsync(AccountModel account)
        {
            const string query = @"
                INSERT INTO accounts (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id)
                VALUES (@AccountId, @Email, @AccountName, @PasswordHash, @IsDeleted, @IsBanned, @Credits, @LastIpAddress, @LastLogin, @CreatedAt, @SessionId)";
            using var conn = CreateConnection();
            await conn.ExecuteAsync(query, account);
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            const string query = "UPDATE accounts SET password = @Password WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync(query, new { Password = newPassword, AccountId = accountId });
            return rows > 0;
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName)
        {
            const string query = "UPDATE accounts SET account_name = @AccountName WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync(query, new { AccountName = accountName, AccountId = accountId });
            return rows > 0;
        }

        public async Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId)
        {
            const string query = "UPDATE accounts SET session_id = @SessionId WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            var rows = await conn.ExecuteAsync(query, new { SessionId = sessionId, AccountId = accountId });
            return rows > 0;
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            const string query = "SELECT 1 FROM accounts WHERE LOWER(email) = LOWER(@Email) LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.ExecuteScalarAsync<int?>(query, new { Email = email });
            return result.HasValue;
        }

        public async Task<bool> IsNameTakenAsync(string accountName)
        {
            const string query = "SELECT 1 FROM accounts WHERE LOWER(account_name) = LOWER(@AccountName) LIMIT 1";
            using var conn = CreateConnection();
            var result = await conn.ExecuteScalarAsync<int?>(query, new { AccountName = accountName });
            return result.HasValue;
        }
    }
}

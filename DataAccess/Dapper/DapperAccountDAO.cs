using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess;

namespace VentusServer.DataAccess.Postgres
{
    public class DapperAccountDAO : IAccountDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DapperAccountDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection CreateConnection()
        {
            var conn = _connectionFactory.CreateConnection();
            LoggerUtil.Log("Database", "Conexión a la base de datos creada.", ConsoleColor.Blue);
            return conn;
        }

        private static string BaseQuery => "SELECT * FROM accounts WHERE {0} LIMIT 1";

        private async Task<AccountModel?> GetAccountByAsync<T>(string fieldName, T value)
        {
            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                LoggerUtil.Log("AccountDAO", $"Valor nulo o vacío para campo {fieldName}", ConsoleColor.Yellow);
                return null;
            }

            if (value is Guid guid && guid == Guid.Empty)
            {
                LoggerUtil.Log("AccountDAO", $"GUID vacío para campo {fieldName}", ConsoleColor.Yellow);
                return null;
            }

            var query = string.Format(BaseQuery, $"{fieldName} = @{fieldName}");
            LoggerUtil.Log("AccountDAO", $"Ejecutando query: {query}", ConsoleColor.Cyan);

            var parameters = new DynamicParameters();
            parameters.Add(fieldName, value);

            using var conn = CreateConnection();
            var rawResult = await conn.QueryFirstOrDefaultAsync(query, parameters);

            if (rawResult == null)
            {
                LoggerUtil.Log("AccountDAO", $"No se encontró cuenta con {fieldName} = {value}", ConsoleColor.Red);
                return null;
            }

            var mapped = MapToAccountModel(rawResult);
            LoggerUtil.Log("AccountDAO", $"Cuenta encontrada: {mapped.AccountId}", ConsoleColor.Green);
            return mapped;
        }

        public Task<AccountModel?> GetAccountByEmailAsync(string email) =>
            GetAccountByAsync("email", email);

        public Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId) =>
            GetAccountByAsync("account_id", accountId);

        public Task<AccountModel?> GetAccountByNameAsync(string accountName) =>
            GetAccountByAsync("account_name", accountName);

        public async Task CreateAccountAsync(AccountModel account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            const string query = @"
                INSERT INTO accounts 
                    (account_id, email, account_name, password, is_deleted, is_banned, credits, last_ip, last_login, created_at, session_id, active_player_id, token_issued_at)
                VALUES 
                    (@AccountId, @Email, @AccountName, @PasswordHash, @IsDeleted, @IsBanned, @Credits, @LastIpAddress, @LastLogin, @CreatedAt, @SessionId, @ActivePlayerId, @TokenIssuedAt)";

            using var conn = CreateConnection();
            await conn.ExecuteAsync(query, account);
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(newPassword)) return false;

            const string query = "UPDATE accounts SET password = @Password WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            return await conn.ExecuteAsync(query, new { Password = newPassword, AccountId = accountId }) > 0;
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(accountName)) return false;

            const string query = "UPDATE accounts SET account_name = @AccountName WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            return await conn.ExecuteAsync(query, new { AccountName = accountName, AccountId = accountId }) > 0;
        }

        public async Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId)
        {
            if (accountId == Guid.Empty || sessionId == Guid.Empty) return false;

            const string query = "UPDATE accounts SET session_id = @SessionId WHERE account_id = @AccountId";
            using var conn = CreateConnection();
            return await conn.ExecuteAsync(query, new { SessionId = sessionId, AccountId = accountId }) > 0;
        }

        public async Task<bool> AccountExistsAsync(Guid accountId)
        {
            const string query = "SELECT 1 FROM accounts WHERE account_id = @AccountId LIMIT 1";
            using var conn = CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(query, new { AccountId = accountId }) != null;
        }

        public async Task<bool> AccountExistsByNameAsync(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            const string query = "SELECT 1 FROM accounts WHERE account_name = @AccountName LIMIT 1";
            using var conn = CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(query, new { AccountName = accountName }) != null;
        }

        public async Task DeleteAccountAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            const string query = "DELETE FROM accounts WHERE email = @Email";
            using var conn = CreateConnection();
            await conn.ExecuteAsync(query, new { Email = email });
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string query = "SELECT 1 FROM accounts WHERE LOWER(email) = LOWER(@Email) LIMIT 1";
            using var conn = CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(query, new { Email = email }) != null;
        }

        public async Task<bool> IsNameTakenAsync(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            const string query = "SELECT 1 FROM accounts WHERE LOWER(account_name) = LOWER(@AccountName) LIMIT 1";
            using var conn = CreateConnection();
            return await conn.ExecuteScalarAsync<int?>(query, new { AccountName = accountName }) != null;
        }

        public async Task<bool> UpdateAccountAsync(AccountModel account)
        {
            if (account == null || account.AccountId == Guid.Empty)
                return false;

            const string query = @"
                UPDATE accounts SET
                    email = @Email,
                    account_name = @AccountName,
                    password = @PasswordHash,
                    is_deleted = @IsDeleted,
                    is_banned = @IsBanned,
                    credits = @Credits,
                    last_ip = @LastIpAddress,
                    last_login = @LastLogin,
                    session_id = @SessionId,
                    active_player_id = @ActivePlayerId,
                    token_issued_at = @TokenIssuedAt
                WHERE account_id = @AccountId";

            using var conn = CreateConnection();
            var result = await conn.ExecuteAsync(query, account);
            return result > 0;
        }

        private AccountModel MapToAccountModel(dynamic data)
        {
            return new AccountModel
            {
                AccountId = data.account_id,
                Email = data.email,
                AccountName = data.account_name,
                PasswordHash = data.password,
                IsDeleted = data.is_deleted,
                IsBanned = data.is_banned,
                Credits = data.credits,
                LastIpAddress = data.last_ip,
                LastLogin = data.last_login,
                CreatedAt = data.created_at,
                SessionId = data.session_id,
                ActivePlayerId = data.active_player_id,
            };
        }
    }
}

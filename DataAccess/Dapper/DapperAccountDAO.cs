using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess;
using VentusServer.Models;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Dapper;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers; // 👈 Import del Mapper

namespace VentusServer.DataAccess.Postgres
{
    public class DapperAccountDAO : BaseDAO, IAccountDAO
    {
        public DapperAccountDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        private async Task<AccountModel?> GetAccountAsync(string query, object parameters, string identifier)
        {
            using var conn = GetConnection();
            var rawResult = await conn.QueryFirstOrDefaultAsync(query, parameters);

            if (rawResult == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ No se encontró cuenta con {identifier}");
                return null;
            }

            var mapped = AccountMapper.Map(rawResult); // 👈 Uso del mapper
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✅ Cuenta encontrada: {mapped.AccountId}");
            return mapped;
        }

        public Task<AccountModel?> GetAccountByEmailAsync(string email) =>
            string.IsNullOrWhiteSpace(email)
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectByEmail, new { Email = email }, $"email = {email}");

        public Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId) =>
            accountId == Guid.Empty
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectById, new { AccountId = accountId }, $"account_id = {accountId}");

        public Task<AccountModel?> GetAccountByNameAsync(string accountName) =>
            string.IsNullOrWhiteSpace(accountName)
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectByName, new { AccountName = accountName }, $"account_name = {accountName}");

        public async Task CreateAccountAsync(AccountModel account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            using var conn = GetConnection();

            var dbEntity = AccountMapper.ToEntity(account);
            await conn.ExecuteAsync(AccountQueries.Insert, dbEntity);
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(newPassword)) return false;

            using var conn = GetConnection();
            return await conn.ExecuteAsync(AccountQueries.UpdatePassword, new { Password = newPassword, AccountId = accountId }) > 0;
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName)
        {
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(accountName)) return false;

            using var conn = GetConnection();
            return await conn.ExecuteAsync(AccountQueries.UpdateAccountName, new { AccountName = accountName, AccountId = accountId }) > 0;
        }

        public async Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId)
        {
            if (accountId == Guid.Empty || sessionId == Guid.Empty) return false;

            using var conn = GetConnection();
            return await conn.ExecuteAsync(AccountQueries.UpdateSessionId, new { SessionId = sessionId, AccountId = accountId }) > 0;
        }
        public async Task<bool> UpdateAccountAsync(AccountModel account)
        {
            if (account == null || account.AccountId == Guid.Empty) return false;
            using var conn = GetConnection();

            var dbEntity = AccountMapper.ToEntity(account);
            return await conn.ExecuteAsync(AccountQueries.UpdateAccount, dbEntity) > 0;
        }

        public async Task<bool> AccountExistsAsync(Guid accountId)
        {
            using var conn = GetConnection();
            return await conn.ExecuteScalarAsync<int?>(AccountQueries.ExistsById, new { AccountId = accountId }) != null;
        }

        public async Task<bool> AccountExistsByNameAsync(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            using var conn = GetConnection();
            return await conn.ExecuteScalarAsync<int?>(AccountQueries.ExistsByName, new { AccountName = accountName }) != null;
        }

        public async Task DeleteAccountAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            using var conn = GetConnection();
            await conn.ExecuteAsync(AccountQueries.DeleteByEmail, new { Email = email });
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            using var conn = GetConnection();
            return await conn.ExecuteScalarAsync<int?>(AccountQueries.IsEmailTaken, new { Email = email }) != null;
        }

        public async Task<bool> IsNameTakenAsync(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            using var conn = GetConnection();
            return await conn.ExecuteScalarAsync<int?>(AccountQueries.IsNameTaken, new { AccountName = accountName }) != null;
        }

        public async Task InitializeAccountsAsync()
        {
            try
            {
                using var conn = GetConnection();
                await conn.ExecuteAsync(AccountQueries.CreateTableQuery);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "✅ Tabla 'accounts' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ Error al crear la tabla 'accounts': {ex.Message}");
            }
        }
    }
}

using System.Data;
using Dapper;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Dapper;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers;

namespace VentusServer.DataAccess.Postgres
{
    public class DapperAccountDAO : BaseDAO, IAccountDAO
    {
        public DapperAccountDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        private async Task<AccountModel?> GetAccountAsync(string query, object parameters, string identifier)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🔍 Ejecutando búsqueda de cuenta con {identifier} {query}...");
            using var conn = GetConnection();

            try
            {
                var rawResult = await conn.QueryFirstOrDefaultAsync(query, parameters);

                if (rawResult == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ No se encontró cuenta con {identifier}");
                    return null;
                }
                var mapped = AccountMapper.Map(rawResult);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✅ Cuenta encontrada con {identifier}: ID = {mapped.AccountId}");
                return mapped;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ Error al obtener cuenta con {identifier}: {ex.Message}");
                return null;
            }
        }

        public Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"📨 Buscando cuenta por email: {email}");
            return string.IsNullOrWhiteSpace(email)
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectByEmail, new { Email = email }, $"email = {email}");
        }

        public Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🆔 Buscando cuenta por ID: {accountId}");
            return accountId == Guid.Empty
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectById, new { AccountId = accountId }, $"account_id = {accountId}");
        }

        public Task<AccountModel?> GetAccountByNameAsync(string accountName)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"👤 Buscando cuenta por nombre: {accountName}");
            return string.IsNullOrWhiteSpace(accountName)
                ? Task.FromResult<AccountModel?>(null)
                : GetAccountAsync(AccountQueries.SelectByName, new { AccountName = accountName }, $"account_name = {accountName}");
        }

        public async Task CreateAccountAsync(AccountModel account)
        {
            if (account == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "❌ No se puede crear una cuenta nula.");
                throw new ArgumentNullException(nameof(account));
            }

            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🛠️ Creando cuenta con email: {account.Email}");
            using var conn = GetConnection();

            try
            {
                var dbEntity = AccountMapper.ToEntity(account);
                await conn.ExecuteAsync(AccountQueries.Insert, dbEntity);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✅ Cuenta creada correctamente: {account.AccountId}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ Error al crear la cuenta: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🔑 Actualizando contraseña para cuenta: {accountId}");
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(newPassword))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "❌ Datos inválidos para actualizar contraseña.");
                return false;
            }

            using var conn = GetConnection();
            var updated = await conn.ExecuteAsync(AccountQueries.UpdatePassword, new { Password = newPassword, AccountId = accountId }) > 0;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, updated
                ? $"✅ Contraseña actualizada correctamente para cuenta: {accountId}"
                : $"⚠️ No se actualizó la contraseña (no se encontró la cuenta?): {accountId}");

            return updated;
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✏️ Actualizando nombre para cuenta: {accountId}");
            if (accountId == Guid.Empty || string.IsNullOrWhiteSpace(accountName))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "❌ Datos inválidos para actualizar nombre.");
                return false;
            }

            using var conn = GetConnection();
            var updated = await conn.ExecuteAsync(AccountQueries.UpdateAccountName, new { AccountName = accountName, AccountId = accountId }) > 0;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, updated
                ? $"✅ Nombre actualizado a '{accountName}' para cuenta: {accountId}"
                : $"⚠️ No se actualizó el nombre para la cuenta: {accountId}");

            return updated;
        }

        public async Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🔁 Actualizando SessionId para cuenta: {accountId}");
            if (accountId == Guid.Empty || sessionId == Guid.Empty)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "❌ Datos inválidos para actualizar SessionId.");
                return false;
            }

            using var conn = GetConnection();
            var updated = await conn.ExecuteAsync(AccountQueries.UpdateSessionId, new { SessionId = sessionId, AccountId = accountId }) > 0;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, updated
                ? $"✅ SessionId actualizado para cuenta: {accountId}"
                : $"⚠️ No se actualizó el SessionId para cuenta: {accountId}");

            return updated;
        }

        public async Task<bool> UpdateAccountAsync(AccountModel account)
        {
            if (account == null || account.AccountId == Guid.Empty)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "❌ Datos inválidos para actualizar cuenta.");
                return false;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"♻️ Actualizando datos de cuenta: {account.AccountId}");
            using var conn = GetConnection();
            var dbEntity = AccountMapper.ToEntity(account);

            var updated = await conn.ExecuteAsync(AccountQueries.UpdateAccount, dbEntity) > 0;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, updated
                ? $"✅ Cuenta actualizada correctamente: {account.AccountId}"
                : $"⚠️ No se actualizó la cuenta: {account.AccountId}");

            return updated;
        }

        public async Task<bool> AccountExistsAsync(Guid accountId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🔎 Verificando existencia de cuenta por ID: {accountId}");
            using var conn = GetConnection();
            var exists = await conn.ExecuteScalarAsync<int?>(AccountQueries.ExistsById, new { AccountId = accountId }) != null;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, exists
                ? $"✅ La cuenta existe: {accountId}"
                : $"❌ La cuenta NO existe: {accountId}");
            return exists;
        }

        public async Task<bool> AccountExistsByNameAsync(string accountName)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🔎 Verificando existencia de cuenta por nombre: {accountName}");
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            using var conn = GetConnection();
            var exists = await conn.ExecuteScalarAsync<int?>(AccountQueries.ExistsByName, new { AccountName = accountName }) != null;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, exists
                ? $"✅ El nombre ya está en uso: {accountName}"
                : $"🆗 El nombre está disponible: {accountName}");
            return exists;
        }

        public async Task DeleteAccountAsync(string email)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"🗑️ Eliminando cuenta con email: {email}");
            if (string.IsNullOrWhiteSpace(email)) return;

            using var conn = GetConnection();
            await conn.ExecuteAsync(AccountQueries.DeleteByEmail, new { Email = email });
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✅ Cuenta eliminada (si existía): {email}");
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"📧 Verificando si el email ya está registrado: {email}");
            if (string.IsNullOrWhiteSpace(email)) return false;

            using var conn = GetConnection();
            var exists = await conn.ExecuteScalarAsync<int?>(AccountQueries.IsEmailTaken, new { Email = email }) != null;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, exists
                ? $"❌ Email ya registrado: {email}"
                : $"🆗 Email disponible: {email}");
            return exists;
        }

        public async Task<bool> IsNameTakenAsync(string accountName)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"📛 Verificando si el nombre ya está registrado: {accountName}");
            if (string.IsNullOrWhiteSpace(accountName)) return false;

            using var conn = GetConnection();
            var exists = await conn.ExecuteScalarAsync<int?>(AccountQueries.IsNameTaken, new { AccountName = accountName }) != null;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, exists
                ? $"❌ Nombre ya registrado: {accountName}"
                : $"🆗 Nombre disponible: {accountName}");
            return exists;
        }

        public async Task InitializeAccountsAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "🧱 Inicializando tabla de cuentas...");
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
        public async Task<List<AccountModel>> GetAllAccountsAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, "📋 Obteniendo todas las cuentas...");
            using var conn = GetConnection();

            try
            {
                var rawResults = await conn.QueryAsync(AccountQueries.SelectAllAccounts);
                var mappedAccounts = rawResults
                    .Select(AccountMapper.Map)
                    .Where(acc => acc != null)
                    .ToList()!; // Materializamos y convertimos en List

                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"✅ Se encontraron {mappedAccounts.Count} cuentas.");
                return mappedAccounts;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperAccountDAO, $"❌ Error al obtener todas las cuentas: {ex.Message}");
                return new List<AccountModel>(); // Retorna lista vacía correctamente
            }
        }


    }
}

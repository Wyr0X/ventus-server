using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess;
using VentusServer.DTOs;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class AccountService : BaseCachedService<AccountModel, Guid>
    {
        private readonly IAccountDAO _accountDao;
        private readonly RoleService _roleService;

        private readonly Dictionary<string, Guid> _emailToIdCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Guid> _nameToIdCache = new(StringComparer.OrdinalIgnoreCase);
        public AccountService(IAccountDAO accountDao, RoleService roleService)
        {
            _accountDao = accountDao;
            _roleService = roleService;
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, "AccountService inicializado.");
        }

        public async Task<List<AccountDTO>> GetAllAccountsAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, "Obteniendo todas las cuentas desde la base de datos...");
            var accounts = await _accountDao.GetAllAccountsAsync();

            var result = new List<AccountDTO>();
            foreach (var account in accounts)
            {
                var role = await _roleService.GetRoleByIdAsync(account.RoleId);
                result.Add(new AccountDTO
                {
                    AccountId = account.AccountId,
                    Email = account.Email,
                    AccountName = account.AccountName,
                    IsBanned = account.IsBanned,
                    CreatedAt = account.CreatedAt,
                    ActivePlayerId = account.ActivePlayerId,
                    RoleName = role?.DisplayName ?? "Desconocido"
                });
            }

            return result;
        }
        protected override async Task<AccountModel?> LoadModelAsync(Guid accountId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cargando cuenta con ID {accountId} desde la base de datos...");

            var account = await _accountDao.GetAccountByAccountIdAsync(accountId);

            if (account != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cuenta encontrada para ID {accountId}.");

                if (!string.IsNullOrEmpty(account.Email))
                    _emailToIdCache[account.Email] = account.AccountId;

                if (!string.IsNullOrEmpty(account.AccountName))
                    _nameToIdCache[account.AccountName] = account.AccountId;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"No se encontró cuenta con ID {accountId}.");
            }

            return account;
        }

        public Task<AccountModel?> GetOrCreateAccountInCacheAsync(Guid accountId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Obteniendo o creando en caché la cuenta con ID {accountId}.");
            return GetOrLoadAsync(accountId);
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Buscando cuenta por email: {email}");

            if (_emailToIdCache.TryGetValue(email, out var id))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Email encontrado en caché con ID: {id}");
                return await GetOrLoadAsync(id);
            }
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Buscando cuenta por email: {email}");

            var account = await _accountDao.GetAccountByEmailAsync(email);
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Buscando cuenta por email: {email}");

            if (account != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cuenta encontrada en DB para email: {email}, ID: {account.AccountId}");
                _emailToIdCache[email] = account.AccountId;
                Set(account.AccountId, account);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"No se encontró cuenta para el email: {email}");
            }

            return account;
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string name)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Buscando cuenta por nombre: {name}");

            if (_nameToIdCache.TryGetValue(name, out var id))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Nombre encontrado en caché con ID: {id}");
                return await GetOrLoadAsync(id);
            }

            var account = await _accountDao.GetAccountByNameAsync(name);
            if (account != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cuenta encontrada en DB para nombre: {name}, ID: {account.AccountId}");
                _nameToIdCache[name] = account.AccountId;
                Set(account.AccountId, account);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"No se encontró cuenta para el nombre: {name}");
            }

            return account;
        }

        public async Task SaveAccountAsync(AccountModel accountModel)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Guardando cuenta: {accountModel.AccountId}");

            var existingByEmail = !string.IsNullOrEmpty(accountModel.Email)
                ? await GetAccountByEmailAsync(accountModel.Email)
                : null;

            if (existingByEmail != null && existingByEmail.AccountId != accountModel.AccountId)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Email {accountModel.Email} ya en uso.", isError: true);
                throw new Exception("Email ya está en uso.");
            }

            var existingByName = !string.IsNullOrEmpty(accountModel.AccountName)
                ? await GetAccountByNameAsync(accountModel.AccountName)
                : null;

            if (existingByName != null && existingByName.AccountId != accountModel.AccountId)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Nombre de cuenta {accountModel.AccountName} ya en uso.", isError: true);
                throw new Exception("Nombre de cuenta ya está en uso.");
            }

            await _accountDao.UpdateAccountAsync(accountModel);
            Set(accountModel.AccountId, accountModel);

            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cuenta actualizada: {accountModel.AccountId}");

            if (!string.IsNullOrEmpty(accountModel.Email))
                _emailToIdCache[accountModel.Email] = accountModel.AccountId;

            if (!string.IsNullOrEmpty(accountModel.AccountName))
                _nameToIdCache[accountModel.AccountName] = accountModel.AccountId;
        }

        public Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Actualizando contraseña para cuenta {accountId}");
            return _accountDao.UpdateAccountPasswordAsync(accountId, newPassword);
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Actualizando nombre para cuenta {accountId} a '{newName}'");

            var updated = await _accountDao.UpdateAccountNameAsync(accountId, newName);
            if (updated)
            {
                var account = await GetOrLoadAsync(accountId);
                if (account != null)
                {
                    account.AccountName = newName;
                    _nameToIdCache[newName] = accountId;
                }
            }

            return updated;
        }

        public async Task<int?> GetActivePlayerAsync(Guid accountId)
        {
            var account = await GetOrLoadAsync(accountId);
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Obteniendo jugador activo para la cuenta {accountId}: {account?.ActivePlayerId}");
            return account?.ActivePlayerId;
        }

        public async Task CreateAccountAsync(AccountModel accountModelToCreate)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Creando cuenta con ID {accountModelToCreate.AccountId}");

            if (!string.IsNullOrEmpty(accountModelToCreate.Email))
            {
                if (await _accountDao.IsEmailTakenAsync(accountModelToCreate.Email))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Email en uso: {accountModelToCreate.Email}", isError: true);
                    throw new Exception("Email ya está en uso.");
                }
            }

            if (!string.IsNullOrEmpty(accountModelToCreate.AccountName))
            {
                if (await _accountDao.IsNameTakenAsync(accountModelToCreate.AccountName))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Nombre en uso: {accountModelToCreate.AccountName}", isError: true);
                    throw new Exception("Nombre de cuenta ya está en uso.");
                }
            }

            await _accountDao.CreateAccountAsync(accountModelToCreate);
            Set(accountModelToCreate.AccountId, accountModelToCreate);

            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Cuenta creada correctamente: {accountModelToCreate.AccountId}");
        }

        public async Task<AccountModel?> UpdateSessionId(Guid accountId, Guid sessionId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AccountService, $"Actualizando SessionId para la cuenta {accountId} a {sessionId}");

            var account = await GetOrCreateAccountInCacheAsync(accountId);
            if (account != null)
            {
                account.SessionId = sessionId;
                await _accountDao.UpdateSessionIdAsync(accountId, sessionId);
            }

            return account;
        }
    }
}

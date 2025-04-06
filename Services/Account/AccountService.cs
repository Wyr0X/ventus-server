using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Postgres;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class AccountService : BaseCachedService<AccountModel, Guid>
    {
        private readonly IAccountDAO _accountDao;
        private readonly Dictionary<string, Guid> _emailToIdCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Guid> _nameToIdCache = new(StringComparer.OrdinalIgnoreCase);

        public AccountService(IAccountDAO accountDao)
        {
            _accountDao = accountDao;
        }

        protected override async Task<AccountModel?> LoadModelAsync(Guid accountId)
        {
            LoggerUtil.Log("AccountService", $"Cargando cuenta desde base de datos: {accountId}", ConsoleColor.DarkCyan);
            var account = await _accountDao.GetAccountByAccountIdAsync(accountId);
            if (account != null)
            {
                LoggerUtil.Log("AccountService", $"Cuenta encontrada: {account.Email}", ConsoleColor.Green);

                if (!string.IsNullOrEmpty(account.Email))
                    _emailToIdCache[account.Email] = account.AccountId;

                if (!string.IsNullOrEmpty(account.AccountName))
                    _nameToIdCache[account.AccountName] = account.AccountId;
            }
            else
            {
                LoggerUtil.Log("AccountService", $"Cuenta no encontrada: {accountId}", ConsoleColor.Yellow);
            }

            return account;
        }

        public async Task<AccountModel?> GetOrCreateAccountInCacheAsync(Guid accountId)
        {
            LoggerUtil.Log("AccountService", $"Obteniendo cuenta en caché o base: {accountId}", ConsoleColor.Cyan);
            return await GetOrLoadAsync(accountId);
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            if (_emailToIdCache.TryGetValue(email, out var id))
            {
                LoggerUtil.Log("AccountService", $"Cache hit para email: {email}", ConsoleColor.Green);
                return await GetOrLoadAsync(id);
            }

            LoggerUtil.Log("AccountService", $"Buscando cuenta por email: {email}", ConsoleColor.DarkCyan);
            var account = await _accountDao.GetAccountByEmailAsync(email);
            if (account != null)
            {
                LoggerUtil.Log("AccountService", $"Cuenta encontrada: {account.Email}", ConsoleColor.Green);
                _emailToIdCache[email] = account.AccountId;
                account.PrintInfo();


                Set(account.AccountId, account);
            }
            else
            {
                LoggerUtil.Log("AccountService", $"Cuenta no encontrada por email: {email}", ConsoleColor.Yellow);
            }

            return account;
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string name)
        {
            if (_nameToIdCache.TryGetValue(name, out var id))
            {
                LoggerUtil.Log("AccountService", $"Cache hit para nombre: {name}", ConsoleColor.Green);
                return await GetOrLoadAsync(id);
            }

            LoggerUtil.Log("AccountService", $"Buscando cuenta por nombre: {name}", ConsoleColor.DarkCyan);
            var account = await _accountDao.GetAccountByNameAsync(name);
            if (account != null)
            {
                _nameToIdCache[name] = account.AccountId;
                Set(account.AccountId, account);
            }
            else
            {
                LoggerUtil.Log("AccountService", $"Cuenta no encontrada por nombre: {name}", ConsoleColor.Yellow);
            }

            return account;
        }

        public async Task SaveAccountAsync(AccountModel accountModel)
        {
            LoggerUtil.Log("AccountService", $"Guardando cuenta: {accountModel.Email}", ConsoleColor.Blue);

            if (!string.IsNullOrEmpty(accountModel.Email))
            {
                var existing = await GetAccountByEmailAsync(accountModel.Email);
                if (existing != null && existing.AccountId != accountModel.AccountId)
                {
                    LoggerUtil.Log("AccountService", $"Email en uso: {accountModel.Email}", ConsoleColor.Red);
                    throw new Exception("Email ya está en uso.");
                }
            }

            if (!string.IsNullOrEmpty(accountModel.AccountName))
            {
                var existing = await GetAccountByNameAsync(accountModel.AccountName);
                if (existing != null && existing.AccountId != accountModel.AccountId)
                {
                    LoggerUtil.Log("AccountService", $"Nombre en uso: {accountModel.AccountName}", ConsoleColor.Red);
                    throw new Exception("Nombre de cuenta ya está en uso.");
                }
            }

            await _accountDao.SaveAccountAsync(accountModel);
            Set(accountModel.AccountId, accountModel);

            if (!string.IsNullOrEmpty(accountModel.Email))
                _emailToIdCache[accountModel.Email] = accountModel.AccountId;

            if (!string.IsNullOrEmpty(accountModel.AccountName))
                _nameToIdCache[accountModel.AccountName] = accountModel.AccountId;

            LoggerUtil.Log("AccountService", $"Cuenta guardada exitosamente: {accountModel.Email}", ConsoleColor.Green);
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            LoggerUtil.Log("AccountService", $"Actualizando contraseña para cuenta: {accountId}", ConsoleColor.Magenta);
            return await _accountDao.UpdateAccountPasswordAsync(accountId, newPassword);
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
            LoggerUtil.Log("AccountService", $"Actualizando nombre de cuenta: {accountId} -> {newName}", ConsoleColor.Magenta);
            var updated = await _accountDao.UpdateAccountNameAsync(accountId, newName);
            if (updated)
            {
                var account = await GetOrLoadAsync(accountId);
                if (account != null)
                {
                    account.AccountName = newName;
                    _nameToIdCache[newName] = accountId;
                    LoggerUtil.Log("AccountService", $"Nombre actualizado en caché: {newName}", ConsoleColor.Green);
                }
            }
            return updated;
        }

        public async Task<int?> GetActivePlayerAsync(Guid accountId)
        {
            LoggerUtil.Log("AccountService", $"Obteniendo jugador activo para cuenta: {accountId}", ConsoleColor.Gray);
            var account = await GetOrLoadAsync(accountId);
            return account?.ActivePlayerId;
        }

        public async Task CreateAccountAsync(AccountModel accountModelToCreate)
        {
            LoggerUtil.Log("AccountService", $"Creando cuenta: {accountModelToCreate.Email}", ConsoleColor.Blue);

            if (!string.IsNullOrEmpty(accountModelToCreate.Email))
            {
                var emailTaken = await _accountDao.IsEmailTakenAsync(accountModelToCreate.Email);
                if (emailTaken)
                {
                    LoggerUtil.Log("AccountService", $"Email ya en uso: {accountModelToCreate.Email}", ConsoleColor.Red);
                    throw new Exception("Email ya está en uso.");
                }
            }

            if (!string.IsNullOrEmpty(accountModelToCreate.AccountName))
            {
                var nameTaken = await _accountDao.IsNameTakenAsync(accountModelToCreate.AccountName);
                if (nameTaken)
                {
                    LoggerUtil.Log("AccountService", $"Nombre ya en uso: {accountModelToCreate.AccountName}", ConsoleColor.Red);
                    throw new Exception("Nombre de cuenta ya está en uso.");
                }
            }

            await _accountDao.CreateAccountAsync(accountModelToCreate);
            Set(accountModelToCreate.AccountId, accountModelToCreate);
            LoggerUtil.Log("AccountService", $"Cuenta creada correctamente: {accountModelToCreate.Email}", ConsoleColor.Green);
        }

        public async Task<AccountModel?> UpdateSessionId(Guid accountId, Guid sessionId)
        {
            LoggerUtil.Log("AccountService", $"Actualizando SessionId para cuenta: {accountId}", ConsoleColor.DarkYellow);
            var account = await GetOrCreateAccountInCacheAsync(accountId);
            if (account != null)
            {
                account.SessionId = sessionId;
                await _accountDao.UpdateSessionIdAsync(accountId, sessionId);
                LoggerUtil.Log("AccountService", $"SessionId actualizado: {sessionId}", ConsoleColor.Green);
            }
            else
            {
                LoggerUtil.Log("AccountService", $"No se encontró cuenta para actualizar SessionId: {accountId}", ConsoleColor.Red);
            }
            return account;
        }
    }
}

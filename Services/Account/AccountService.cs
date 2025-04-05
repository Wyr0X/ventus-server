using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Postgres;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class AccountService : BaseCachedService<AccountModel, Guid>
    {
        private readonly PostgresAccountDAO _accountDao;
        private readonly Dictionary<string, Guid> _emailToIdCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Guid> _nameToIdCache = new(StringComparer.OrdinalIgnoreCase);

        public AccountService(PostgresAccountDAO accountDao)
        {
            _accountDao = accountDao;
        }

        // Método requerido por BaseCachedService
        protected override async Task<AccountModel?> LoadModelAsync(Guid accountId)
        {
            var account = await _accountDao.GetAccountByAccountIdAsync(accountId);
            if (account != null)
            {
                // Al cargar desde DB, cacheamos también los alias
                if (!string.IsNullOrEmpty(account.Email))
                    _emailToIdCache[account.Email] = account.AccountId;

                if (!string.IsNullOrEmpty(account.AccountName))
                    _nameToIdCache[account.AccountName] = account.AccountId;
            }

            return account;
        }

        public async Task<AccountModel?> GetOrCreateAccountInCacheAsync(Guid accountId)
        {
            return await GetOrLoadAsync(accountId);
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            if (_emailToIdCache.TryGetValue(email, out var id))
            {
                return await GetOrLoadAsync(id);
            }

            var account = await _accountDao.GetAccountByEmailAsync(email);
            if (account != null)
            {
                _emailToIdCache[email] = account.AccountId;
                Set(account.AccountId, account);
            }

            return account;
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string name)
        {
            if (_nameToIdCache.TryGetValue(name, out var id))
            {
                return await GetOrLoadAsync(id);
            }

            var account = await _accountDao.GetAccountByNameAsync(name);
            if (account != null)
            {
                _nameToIdCache[name] = account.AccountId;
                Set(account.AccountId, account);
            }

            return account;
        }

        public async Task SaveAccountAsync(AccountModel accountModel)
        {
            await _accountDao.SaveAccountAsync(accountModel);
            Set(accountModel.AccountId, accountModel);

            if (!string.IsNullOrEmpty(accountModel.Email))
                _emailToIdCache[accountModel.Email] = accountModel.AccountId;

            if (!string.IsNullOrEmpty(accountModel.AccountName))
                _nameToIdCache[accountModel.AccountName] = accountModel.AccountId;
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            return await _accountDao.UpdateAccountPasswordAsync(accountId, newPassword);
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
            var updated = await _accountDao.UpdateAccountNameAsync(accountId, newName);
            if (updated)
            {
                // Actualizar en caché si ya existe
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
            return account?.ActivePlayerId;
        }
        
    }
}

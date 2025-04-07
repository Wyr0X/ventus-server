// ... using statements

using VentusServer.DataAccess;

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
            var account = await _accountDao.GetAccountByAccountIdAsync(accountId);
            if (account != null)
            {
                if (!string.IsNullOrEmpty(account.Email))
                    _emailToIdCache[account.Email] = account.AccountId;

                if (!string.IsNullOrEmpty(account.AccountName))
                    _nameToIdCache[account.AccountName] = account.AccountId;
            }
            return account;
        }

        public Task<AccountModel?> GetOrCreateAccountInCacheAsync(Guid accountId)
            => GetOrLoadAsync(accountId);

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
            if (_emailToIdCache.TryGetValue(email, out var id))
                return await GetOrLoadAsync(id);

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
                return await GetOrLoadAsync(id);

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
            var existingByEmail = !string.IsNullOrEmpty(accountModel.Email)
                ? await GetAccountByEmailAsync(accountModel.Email)
                : null;

            if (existingByEmail != null && existingByEmail.AccountId != accountModel.AccountId)
                throw new Exception("Email ya está en uso.");

            var existingByName = !string.IsNullOrEmpty(accountModel.AccountName)
                ? await GetAccountByNameAsync(accountModel.AccountName)
                : null;

            if (existingByName != null && existingByName.AccountId != accountModel.AccountId)
                throw new Exception("Nombre de cuenta ya está en uso.");

            await _accountDao.UpdateAccountAsync(accountModel);
            Set(accountModel.AccountId, accountModel);

            if (!string.IsNullOrEmpty(accountModel.Email))
                _emailToIdCache[accountModel.Email] = accountModel.AccountId;

            if (!string.IsNullOrEmpty(accountModel.AccountName))
                _nameToIdCache[accountModel.AccountName] = accountModel.AccountId;
        }

        public Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
            => _accountDao.UpdateAccountPasswordAsync(accountId, newPassword);

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
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
            return account?.ActivePlayerId;
        }

        public async Task CreateAccountAsync(AccountModel accountModelToCreate)
        {
            if (!string.IsNullOrEmpty(accountModelToCreate.Email))
            {
                if (await _accountDao.IsEmailTakenAsync(accountModelToCreate.Email))
                    throw new Exception("Email ya está en uso.");
            }

            if (!string.IsNullOrEmpty(accountModelToCreate.AccountName))
            {
                if (await _accountDao.IsNameTakenAsync(accountModelToCreate.AccountName))
                    throw new Exception("Nombre de cuenta ya está en uso.");
            }

            await _accountDao.CreateAccountAsync(accountModelToCreate);
            Set(accountModelToCreate.AccountId, accountModelToCreate);
        }

        public async Task<AccountModel?> UpdateSessionId(Guid accountId, Guid sessionId)
        {
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

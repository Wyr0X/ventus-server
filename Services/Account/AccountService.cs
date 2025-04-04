using System;
using System.Threading.Tasks;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class AccountService
    {
        private readonly PostgresAccountDAO _accountDao;

        public AccountService(PostgresAccountDAO accountDao)
        {
            _accountDao = accountDao;
        }

        public async Task<AccountModel?> GetAccountByEmailAsync(string email)
        {
          return await _accountDao.GetAccountByEmailAsync(email);
        }

        public async Task<AccountModel?> GetAccountByIdAsync(Guid accountId)
        {
            return await _accountDao.GetAccountByAccountIdAsync(accountId);
        }

        public async Task<AccountModel?> GetAccountByNameAsync(string name)
        {
            return await _accountDao.GetAccountByNameAsync(name);
        }

        public async Task SaveAccountAsync(AccountModel accountModel)
        {
            await _accountDao.SaveAccountAsync(accountModel);
        }

        public async Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword)
        {
            return await _accountDao.UpdateAccountPasswordAsync(accountId, newPassword);
        }

        public async Task<bool> UpdateAccountNameAsync(Guid accountId, string newName)
        {
            return await _accountDao.UpdateAccountNameAsync(accountId, newName);
        }

        public async Task<int?> GetActivePlayerAsync(Guid accountId)
        {

            AccountModel? account = await _accountDao.GetAccountByAccountIdAsync(accountId);
            return account != null ? account.ActivePlayer : null;
        }


       
    }
}

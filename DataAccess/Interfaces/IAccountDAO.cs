using System;
using System.Threading.Tasks;

namespace VentusServer.DataAccess
{
    public interface IAccountDAO
    {
        Task<AccountModel?> GetAccountByEmailAsync(string email);
        Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId);
        Task<AccountModel?> GetAccountByNameAsync(string accountName);
        Task SaveAccountAsync(AccountModel account);
        Task DeleteAccountAsync(string email);
        Task<bool> AccountExistsAsync(string accountId);
        Task<bool> AccountExistsByNameAsync(string accountName);
        Task CreateAccountAsync(AccountModel account);
        Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword);
        Task<bool> UpdateAccountNameAsync(Guid accountId, string accountName);
        Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsNameTakenAsync(string accountName);
    }
}

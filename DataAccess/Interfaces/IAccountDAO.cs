using System;
using System.Threading.Tasks;

namespace VentusServer.DataAccess
{
    public interface IAccountDAO
    {
        // CREATE
        Task CreateAccountAsync(AccountModel account);

        // READ
        Task<AccountModel?> GetAccountByEmailAsync(string email);
        Task<AccountModel?> GetAccountByAccountIdAsync(Guid accountId);
        Task<AccountModel?> GetAccountByNameAsync(string accountName);

        // EXISTS / VALIDATION
        Task<bool> AccountExistsAsync(Guid accountId);
        Task<bool> AccountExistsByNameAsync(string accountName);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsNameTakenAsync(string accountName);

        // UPDATE
        Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword);
        Task<bool> UpdateAccountNameAsync(Guid accountId, string newAccountName);
        Task<bool> UpdateSessionIdAsync(Guid accountId, Guid sessionId);
        Task<bool> UpdateAccountAsync(AccountModel account);

        Task<List<AccountModel>> GetAllAccountsAsync();

        // DELETE
        Task DeleteAccountAsync(string email);
    }
}

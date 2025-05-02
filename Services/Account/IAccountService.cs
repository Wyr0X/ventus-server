using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DTOs;
using VentusServer.Models;

namespace VentusServer.Services
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetAllAccountsAsync();
        Task<AccountModel?> GetOrCreateAccountInCacheAsync(Guid accountId);
        Task<AccountModel?> GetAccountByEmailAsync(string email);
        Task<AccountModel?> GetAccountByNameAsync(string name);
        Task SaveAccountAsync(AccountModel accountModel);
        Task<bool> UpdateAccountPasswordAsync(Guid accountId, string newPassword);
        Task<bool> UpdateAccountNameAsync(Guid accountId, string newName);
        Task<int?> GetActivePlayerAsync(Guid accountId);
        Task CreateAccountAsync(AccountModel accountModelToCreate);
        Task<AccountModel?> UpdateSessionId(Guid accountId, Guid sessionId);
    }
}

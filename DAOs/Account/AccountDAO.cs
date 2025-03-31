public interface IAccountDAO
{
    Task<Account?> GetAccountByEmailAsync(string email);
    Task<Account?> GetAccountByUserIdAsync(string userId);
    Task<Account?> GetAccountByNameAsync(string Name); // Nuevo método
    Task SaveAccountAsync(Account account);
    Task DeleteAccountAsync(string email);
    Task<bool> AccountExistsAsync(string userId);
    Task CreateAccountAsync(Account account); // Nuevo método para crear una cuenta
    Task<bool> AccountExistsByNameAsync(string Name); // Nuevo método para verificar si existe un Name
}

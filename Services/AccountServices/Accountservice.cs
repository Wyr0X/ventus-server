using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VentusServer.Database;
using VentusServer.Database.Entities;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class AccountService
    {
        private readonly PostgresDbContext _dbContext;

        public AccountService(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Obtener cuenta por User ID (Firebase UID)
        public async Task<AccountEntity?> GetAccountByIdAsync(string userId)
        {
            return await _dbContext.Accounts
                .Include(a => a.Players)  // Incluir la relación con Players (jugadores)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        // Crear nueva cuenta
        public async Task<AccountEntity> CreateAccountAsync(string userId, string email, string name, string ip, string playerName)
        {
            var newAccount = new AccountEntity
            {
                UserId = userId,
                Email = email,
                Name = name,
                LastIp = ip,
                CreatedAt = DateTime.UtcNow
            };
    
            // Crear el nuevo personaje (jugador) al mismo tiempo que la cuenta
            var newPlayer = new PlayerEntity
            {
                Name = playerName,
                AccountId = newAccount.UserId,  // Asociar el jugador a la cuenta
                CreatedAt = DateTime.UtcNow,
                Level = 1,  // Nivel inicial, por ejemplo
                Status = "Active"  // Status inicial
            };

            newAccount.Players.Add(newPlayer);  // Agregar el jugador a la lista de la cuenta

            _dbContext.Accounts.Add(newAccount);  // Agregar la cuenta a la base de datos
            await _dbContext.SaveChangesAsync();  // Guardar cambios en la base de datos

            return newAccount;
        }

        // Obtener jugadores de una cuenta
        public async Task<List<PlayerEntity>> GetPlayersByAccountIdAsync(string userId)
        {
            var account = await _dbContext.Accounts
                .Include(a => a.Players)
                .FirstOrDefaultAsync(a => a.UserId == userId);

            return account?.Players ?? new List<PlayerEntity>();
        }

        // Actualizar información de la cuenta
        public async Task<bool> UpdateAccountAsync(AccountEntity account)
        {
            _dbContext.Accounts.Update(account);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // Eliminar cuenta (Soft Delete)
        public async Task<bool> DeleteAccountAsync(string userId)
        {
            var account = await GetAccountByIdAsync(userId);
            if (account == null) return false;

            account.IsDeleted = true;
            _dbContext.Accounts.Update(account);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

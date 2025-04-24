using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VentusServer.DataAccess;
using VentusServer.Models;

namespace VentusServer.Seeders
{
    public class AccountSeeder
    {
        private readonly IAccountDAO _accountDAO;
        private readonly PasswordService _passwordService; // Asegurate de tener uno

        public AccountSeeder(IAccountDAO accountDAO, PasswordService passwordService)
        {
            _accountDAO = accountDAO;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            var email = "pedronicolasabba@gmail.com";
            var plainPassword = "asdasd123";
            var hashedPassword = _passwordService.HashPassword(plainPassword);
            var exists = await _accountDAO.IsEmailTakenAsync(email);
            if (exists)
            {
                Console.WriteLine("La cuenta ya existe. Seed cancelado.");
                return;
            }

            var account = new AccountModel
            {
                AccountId = Guid.NewGuid(),
                Email = email,
                PasswordHash = hashedPassword,
                AccountName = "pedronicolasabba",
                CreatedAt = DateTime.UtcNow,
                Credits = 0,
                RoleId = "owner",
                LastLogin = null,
                LastIpAddress = "localhost",
                SessionId = Guid.Empty
            };

            await _accountDAO.CreateAccountAsync(account);
            Console.WriteLine("Cuenta seed creada exitosamente.");
        }
    }
}

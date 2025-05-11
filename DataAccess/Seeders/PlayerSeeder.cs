using System;
using System.Threading.Tasks;
using Game.DTOs;
using Game.Models;
using VentusServer.Services;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.Seeders
{
    public class PlayerSeeder
    {
        private readonly IPlayerDAO _playerDAO;
        private readonly IAccountDAO _accountDAO;

        public PlayerSeeder(IPlayerDAO playerDAO, IAccountDAO accountDAO)
        {
            _playerDAO = playerDAO;
            _accountDAO = accountDAO;
        }

        public async Task SeedAsync()
        {
            // var email = "pedronicolasabba@gmail.com";
            // var account = await _accountDAO.GetAccountByEmailAsync(email);
            // if (account == null)
            // {
            //     Console.WriteLine("[Seeder] Cuenta no encontrada.");
            //     return;
            // }

            // var createPlayerDTO = new CreatePlayerDTO
            // {
            //     Name = "SirPedro",
            //     Gender = Gender.Male,
            //     Race = Race.Elfo,
            //     Class = CharacterClass.Warrior
            // };

            // var player = await _playerDAO.CreatePlayerAsync(account.AccountId, createPlayerDTO);

            // if (player != null)
            // {
            //     Console.WriteLine($"[Seeder] Personaje '{player.Name}' creado para la cuenta {account.Email}");
            // }
            // else
            // {
            //     Console.WriteLine("[Seeder] Error al crear el personaje.");
            // }
        }
    }
}

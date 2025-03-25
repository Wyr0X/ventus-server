using System;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.Database;
using VentusServer.Models;

namespace Game.Services
{
    public class PlayerEconomyService : PlayerModuleService<PlayerEconomy, PlayerEconomyEntity>
    {
        private readonly PostgresDbContext _context;

        public PlayerEconomyService(PostgresDbContext context, ICacheService cache) 
            : base(new PlayerEconomyRepository(context), cache) // Inyección del repositorio y cache
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Método para añadir oro al jugador
        public async Task AddGoldAsync(int playerId, decimal amount)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            playerEconomy.AddGold(amount);  // Lógica de negocio para agregar oro

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Método para restar oro del jugador
        public async Task SubtractGoldAsync(int playerId, decimal amount)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (playerEconomy.Gold < amount)
                throw new InvalidOperationException("Insufficient gold to subtract.");

            playerEconomy.SubtractGold(amount);  // Lógica de negocio para restar oro

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Transferir oro al banco
        public async Task TransferGoldToBankAsync(int playerId, decimal amount)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            playerEconomy.TransferGoldToBank(amount);  // Lógica de negocio para transferir oro

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Transferir oro desde el banco
        public async Task TransferGoldFromBankAsync(int playerId, decimal amount)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            playerEconomy.TransferGoldFromBank(amount);  // Lógica de negocio para transferir oro desde el banco

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Actualizar hambre y sed
        public async Task UpdateNeedsAsync(int playerId, int hungerIncrease, int thirstIncrease)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            if (hungerIncrease < 0 || thirstIncrease < 0)
                throw new ArgumentException("Hunger and thirst increases must be non-negative.");

            playerEconomy.UpdateNeeds(hungerIncrease, thirstIncrease);  // Lógica de negocio para aumentar hambre y sed

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Regenerar hambre y sed automáticamente
        public async Task AutoRegenerateNeedsAsync(int playerId)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            playerEconomy.AutoRegenerateNeeds();  // Lógica de negocio para regenerar hambre y sed automáticamente

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }

        // Verificar si el jugador puede continuar
        public async Task<bool> CanContinueAsync(int playerId)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            return playerEconomy.CanContinue();  // Lógica de negocio para verificar si puede continuar
        }

        // Resetear la economía del jugador
        public async Task ResetEconomyAsync(int playerId)
        {
            var playerEconomy = await GetPlayerModuleAsync(playerId);  // Usando la función genérica

            playerEconomy.ResetEconomy();  // Lógica de negocio para resetear la economía

            await SavePlayerModuleAsync(playerEconomy);  // Usando la función genérica
        }
    }
}

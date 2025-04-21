using System;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Models;
using System.Collections.Concurrent;

namespace VentusServer.Services
{
    public class PlayerStatsService
    {
        private readonly IPlayerStatsDAO _playerStatsDAO;
        private readonly ConcurrentDictionary<int, PlayerStatsModel> _cache = new ConcurrentDictionary<int, PlayerStatsModel>(); // Cache de estadísticas por jugador

        public PlayerStatsService(IPlayerStatsDAO playerStatsDAO)
        {
            _playerStatsDAO = playerStatsDAO;
        }

        private async Task<PlayerStatsModel> GetOrLoadPlayerStatsAsync(int playerId)
        {
            if (_cache.TryGetValue(playerId, out var playerStats))
            {
                return playerStats;
            }

            // Si no está en caché, lo cargamos desde la base de datos
            playerStats = await _playerStatsDAO.GetPlayerStatsByIdAsync(playerId);
            if (playerStats != null)
            {
                _cache[playerId] = playerStats; // Guardamos en caché
            }

            return playerStats;
        }

        public async Task<PlayerStatsModel> GetPlayerStatsAsync(int playerId)
        {
            return await GetOrLoadPlayerStatsAsync(playerId);
        }
        public async Task<PlayerStatsModel> LoadPlayerStatsInModel(PlayerModel player)
        {
            var playerStats = await GetPlayerStatsAsync(player.Id);
            player.Stats = playerStats;
            return playerStats;
        }
        public async Task SavePlayerStatsAsync(PlayerStatsModel playerStats)
        {
            try
            {
                Console.WriteLine($"LAST UPDATED {playerStats.LastUpdated}");
                await _playerStatsDAO.SavePlayerStatsAsync(playerStats);
                _cache[playerStats.PlayerId] = playerStats; // Actualizamos la cache
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar las estadísticas del jugador: {ex.Message}");
            }
        }

        public async Task<PlayerStatsModel?> CreateDefaultPlayerStatsAsync(int playerId, CreatePlayerDTO createPlayerDTO)
        {
            // Crear las estadísticas por defecto usando el factory
            var playerStats = PlayerStatsFactory.CreateDefaultPlayerStats(createPlayerDTO.Race, createPlayerDTO.Gender);
            playerStats.PlayerId = playerId; // Asignamos el ID del jugador

            // Guardar las estadísticas del jugador recién creado

            await SavePlayerStatsAsync(playerStats);
            return playerStats;
        }

        // Métodos para actualizar las estadísticas del jugador...
        public async Task UpdateGoldAsync(int playerId, int goldDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.Gold += goldDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateBankGoldAsync(int playerId, int bankGoldDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.BankGold += bankGoldDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateFreeSkillPointsAsync(int playerId, int skillPointsDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.FreeSkillPoints += skillPointsDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateHpAsync(int playerId, int hpDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            // Aseguramos que la salud no sea mayor que el máximo
            playerStats.Hp = Math.Clamp(playerStats.Hp + hpDelta, 0, playerStats.MaxHp); // Evita que la salud exceda el máximo
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateMpAsync(int playerId, int mpDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            // Aseguramos que el maná no sea mayor que el máximo
            playerStats.Mp = Math.Clamp(playerStats.Mp + mpDelta, 0, playerStats.MaxMp); // Evita que el maná exceda el máximo
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateSpAsync(int playerId, int spDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            // Aseguramos que los puntos de acción no sean mayores que el máximo
            playerStats.Sp = Math.Clamp(playerStats.Sp + spDelta, 0, playerStats.MaxSp); // Evita que los puntos de acción excedan el máximo
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateHungerAsync(int playerId, int hungerDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            // Aseguramos que el hambre no sea negativa, si quieres un máximo puedes agregar un límite
            playerStats.Hunger = Math.Max(0, playerStats.Hunger + hungerDelta);
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateThirstAsync(int playerId, int thirstDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            // Aseguramos que la sed no sea negativa, si quieres un máximo puedes agregar un límite
            playerStats.Thirst = Math.Max(0, playerStats.Thirst + thirstDelta);
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateKilledNpcsAsync(int playerId, int killedNpcsDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.KilledNpcs += killedNpcsDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateKilledUsersAsync(int playerId, int killedUsersDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.KilledUsers += killedUsersDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task UpdateDeathsAsync(int playerId, int deathsDelta)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            playerStats.Deaths += deathsDelta;
            await SavePlayerStatsAsync(playerStats);
        }

        public async Task DeletePlayerStatsAsync(int playerId)
        {
            var playerStats = await GetOrLoadPlayerStatsAsync(playerId);
            if (playerStats == null) return;

            await _playerStatsDAO.DeletePlayerStatsAsync(playerId);
            _cache.TryRemove(playerId, out _); // Elimina de la cache
        }
    }
}

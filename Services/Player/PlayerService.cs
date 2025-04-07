using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.Services
{
    public class PlayerService : BaseCachedService<PlayerModel, int>
    {
        private readonly IPlayerDAO _playerDAO;
        private readonly PlayerStatsService _playerStatsService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly Dictionary<string, int> _nameToIdCache = new();

        public PlayerService(IPlayerDAO playerDAO, PlayerLocationService playerLocationService,
            PlayerStatsService playerStatsService)
        {
            _playerDAO = playerDAO;
            _playerLocationService = playerLocationService;
            _playerStatsService = playerStatsService;
        }

        // =============================
        // CRUD BÁSICO
        // =============================

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            try
            {
                return await _playerDAO.GetPlayerByIdAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener el jugador: {ex.Message}");
                return null;
            }
        }

        public async Task SavePlayerAsync(PlayerModel player)
        {
            try
            {
                var existingPlayer = await _playerDAO.GetPlayerByNameAsync(player.Name);
                if (existingPlayer != null && existingPlayer.Id != player.Id)
                {
                    Console.WriteLine($"⚠️ Ya existe un jugador con el nombre '{player.Name}'.");
                    return;
                }

                await _playerDAO.SavePlayerAsync(player);
                _nameToIdCache[player.Name] = player.Id;
                Console.WriteLine("✅ Jugador guardado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar el jugador: {ex.Message}");
            }
        }

        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            try
            {
                var deleted = await _playerDAO.DeletePlayerAsync(playerId);
                if (deleted) Console.WriteLine("✅ Jugador eliminado correctamente.");
                return deleted;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar el jugador: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            try
            {
                return await _playerDAO.PlayerExistsAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar la existencia del jugador: {ex.Message}");
                return false;
            }
        }

        // =============================
        // CREACIÓN DE JUGADOR
        // =============================

        public async Task<PlayerModel?> CreatePlayer(Guid accountId, string name, string gender, string race, string playerClass)
        {
            try
            {
                bool nameExists = await _playerDAO.PlayerNameExistsAsync(name);
                if (nameExists)
                {
                    Console.WriteLine($"⚠️ Ya existe un jugador con el nombre '{name}'.");
                    return null;
                }
                    Console.WriteLine($"Aca {accountId}'.");

                var player = await _playerDAO.CreatePlayerAsync(accountId, name, gender, race, playerClass);
                await _playerLocationService.CreateDefaultPlayerLocation(player);
           //     await _playerLocationService.CreateDefaultPlayerLocation(player);

                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear el jugador: {ex.Message}");
                return null;
            }
        }

        // =============================
        // CONSULTAS
        // =============================

        public async Task<List<PlayerModel>> GetAllPlayers()
        {
            return await _playerDAO.GetAllPlayersAsync();
        }

        public async Task<List<PlayerModel>> GetPlayerWithCompleteInfo()
        {
            return await _playerDAO.GetAllPlayersAsync(); // Puede ajustarse si se necesita info extra
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountId(Guid accountId)
        {
            return await _playerDAO.GetPlayersByAccountIdAsync(accountId);
        }

        public async Task<PlayerModel?> GetPlayerByName(string name)
        {
            if (_nameToIdCache.TryGetValue(name, out int cachedId))
            {
                return await GetOrLoadAsync(cachedId);
            }

            var player = await _playerDAO.GetPlayerByNameAsync(name);
            if (player != null)
            {
                Set(player.Id, player);
                _nameToIdCache[name] = player.Id;
            }

            return player;
        }

        // =============================
        // CACHE
        // =============================

        public async Task<PlayerModel?> GetOrCreatePlayerInCacheAsync(int playerId)
        {
            var cachedPlayer = GetIfLoaded(playerId);
            if (cachedPlayer != null)
                return cachedPlayer;

            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            if (player != null)
            {
                Set(player.Id, player);
                _nameToIdCache[player.Name] = playerId;
            }

            return player;
        }

        protected override async Task<PlayerModel?> LoadModelAsync(int playerId)
        {
            try
            {
                var player = await _playerDAO.GetPlayerByIdAsync(playerId);
                if (player != null)
                {
                    _nameToIdCache[player.Name] = player.Id;
                }
                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar el jugador desde LoadModelAsync: {ex.Message}");
                return null;
            }
        }
    }
}

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
        private readonly PlayerInventoryService _playerInventoryService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly Dictionary<string, int> _nameToIdCache = new();
        private readonly AccountService _accountService;
        public PlayerService(IPlayerDAO playerDAO, PlayerLocationService playerLocationService,
            PlayerStatsService playerStatsService, PlayerInventoryService playerInventoryService, AccountService accountService)
        {
            _playerDAO = playerDAO;
            _playerLocationService = playerLocationService;
            _playerInventoryService = playerInventoryService;
            _playerStatsService = playerStatsService;
            _accountService = accountService;
        }

        // =============================
        // CRUD BÁSICO
        // =============================

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId, PlayerModuleOptions? options = null)
        {
            Console.WriteLine($"🔍 Buscando jugador por ID: {playerId}");
            var player = await GetOrCreatePlayerInCacheAsync(playerId);
            if (player == null)
            {
                Console.WriteLine($"⚠️ No se encontró el jugador con ID {playerId}.");
                return null;
            }

            options ??= new PlayerModuleOptions();
            await LoadPlayerModulesAsync(player, options);

            return player;
        }

        public async Task SavePlayerAsync(PlayerModel player)
        {
            try
            {
                Console.WriteLine($"💾 Guardando jugador: {player.Name} (ID: {player.Id})");
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
                Console.WriteLine($"🗑️ Eliminando jugador con ID: {playerId}");
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
                Console.WriteLine($"🔍 Verificando existencia del jugador ID: {playerId}");
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

        public async Task<PlayerModel?> CreatePlayer(Guid accountId, CreatePlayerDTO createPlayerDTO)
        {
            try
            {
                Console.WriteLine($"🛠️ Creando jugador '{createPlayerDTO.Name}' para cuenta {accountId}");
                bool nameExists = await _playerDAO.PlayerNameExistsAsync(createPlayerDTO.Name);
                if (nameExists)
                {
                    Console.WriteLine($"⚠️ Ya existe un jugador con el nombre '{createPlayerDTO.Name}'.");
                    return null;
                }

                var player = await _playerDAO.CreatePlayerAsync(accountId, createPlayerDTO);
                var location = await _playerLocationService.CreateDefaultPlayerLocation(player);
                var stats = await _playerStatsService.CreateDefaultPlayerStatsAsync(player.Id, createPlayerDTO);
                var inventory = await _playerInventoryService.CreateDefaultInventory(player);
                player.Location = location;
                player.Stats = stats;
                // player.Inventory = inventory;
                Console.WriteLine($"✅ Jugador '{player.Name}' creado exitosamente con ID {player.Location}.");
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
            Console.WriteLine("📄 Obteniendo todos los jugadores...");
            return await _playerDAO.GetAllPlayersAsync();
        }

        public async Task<List<PlayerModel>> GetPlayerWithCompleteInfo()
        {
            Console.WriteLine("📄 Obteniendo todos los jugadores con información completa (simple)...");
            return await _playerDAO.GetAllPlayersAsync(); // Puede ajustarse si se necesita info extra
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountId(Guid accountId, PlayerModuleOptions? options = null)
        {
            Console.WriteLine($"📄 Obteniendo jugadores de la cuenta: {accountId}");
            var players = await _playerDAO.GetPlayersByAccountIdAsync(accountId);
            options ??= new PlayerModuleOptions();

            foreach (var player in players)
            {
                Set(player.Id, player);
                _nameToIdCache[player.Name] = player.Id;
            }

            var loadTasks = players.Select(player => LoadPlayerModulesAsync(player, options));
            await Task.WhenAll(loadTasks);

            return players;
        }

        public async Task<PlayerModel?> GetPlayerByName(string name)
        {
            Console.WriteLine($"🔍 Buscando jugador por nombre: {name}");
            if (_nameToIdCache.TryGetValue(name, out int cachedId))
            {
                Console.WriteLine($"📦 Jugador encontrado en caché: {cachedId}");
                return await GetOrLoadAsync(cachedId);
            }

            var player = await _playerDAO.GetPlayerByNameAsync(name);
            if (player != null)
            {
                Console.WriteLine($"✅ Jugador '{name}' encontrado en base de datos con ID {player.Id}");
                Set(player.Id, player);
                _nameToIdCache[name] = player.Id;
            }
            else
            {
                Console.WriteLine($"⚠️ No se encontró el jugador '{name}'");
            }

            return player;
        }

        public async Task<AccountModel?> GetAccountByPlayerIdAsync(int playerId)
        {
            Console.WriteLine($"🔍 Obteniendo cuenta asociada al jugador ID: {playerId}");
            var player = await GetPlayerByIdAsync(playerId);
            if (player == null)
            {
                Console.WriteLine($"⚠️ No se pudo obtener la cuenta porque el jugador no existe.");
                return null;
            }

            return await _accountService.GetOrCreateAccountInCacheAsync(player.AccountId);
        }

        // =============================
        // CACHE
        // =============================

        public async Task<PlayerModel?> GetOrCreatePlayerInCacheAsync(int playerId)
        {
            var cachedPlayer = GetIfLoaded(playerId);
            if (cachedPlayer != null)
            {
                Console.WriteLine($"📦 Jugador ID {playerId} recuperado de caché.");
                return cachedPlayer;
            }

            Console.WriteLine($"📥 Jugador ID {playerId} no estaba en caché. Cargando desde base de datos...");
            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            if (player != null)
            {
                Set(player.Id, player);
                _nameToIdCache[player.Name] = playerId;
                Console.WriteLine($"✅ Jugador ID {playerId} cargado y cacheado.");
            }
            else
            {
                Console.WriteLine($"⚠️ No se encontró el jugador con ID {playerId} en la base de datos.");
            }

            return player;
        }

        protected override async Task<PlayerModel?> LoadModelAsync(int playerId)
        {
            try
            {
                Console.WriteLine($"📤 Cargando modelo del jugador ID {playerId}...");
                var player = await _playerDAO.GetPlayerByIdAsync(playerId);
                if (player != null)
                {
                    _nameToIdCache[player.Name] = player.Id;
                    Console.WriteLine($"✅ Modelo del jugador ID {playerId} cargado.");
                }
                else
                {
                    Console.WriteLine($"⚠️ No se encontró el modelo del jugador ID {playerId}.");
                }
                return player;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al cargar el jugador desde LoadModelAsync: {ex.Message}");
                return null;
            }
        }

        private async Task LoadPlayerModulesAsync(PlayerModel player, PlayerModuleOptions options)
        {
            var tasks = new List<Task>();
            Console.WriteLine($"📊 Cargando modulos {player.Name} (ID: {player.Id})...");

            if (options.IncludeStats)
            {
                Console.WriteLine($"📊 Cargando estadísticas para el jugador {player.Name} (ID: {player.Id})...");
                tasks.Add(_playerStatsService.LoadPlayerStatsInModel(player)
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            Console.WriteLine($"✅ Estadísticas cargadas para {player.Name}.");
                        else
                            Console.WriteLine($"❌ Error al cargar estadísticas para {player.Name}: {task.Exception?.Message}");
                    }));
            }

            if (options.IncludeInventory)
            {
                Console.WriteLine($"🎒 Cargando inventario para el jugador {player.Name} (ID: {player.Id})...");
                tasks.Add(_playerInventoryService.LoadPlayerInventoryInModule(player)
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            Console.WriteLine($"✅ Inventario cargado para {player.Name}.");
                        else
                            Console.WriteLine($"❌ Error al cargar inventario para {player.Name}: {task.Exception?.Message}");
                    }));
            }

            if (options.IncludeLocation)
            {
                Console.WriteLine($"📍 Cargando ubicación para el jugador {player.Name} (ID: {player.Id})...");
                tasks.Add(_playerLocationService.LoadPlayerLocationInModel(player)
                    .ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            Console.WriteLine($"✅ Ubicación cargada para {player.Name}.");
                        else
                            Console.WriteLine($"❌ Error al cargar ubicación para {player.Name}: {task.Exception?.Message}");
                    }));
            }

            await Task.WhenAll(tasks);
        }

    }
}

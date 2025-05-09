using Game.Models;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.Services
{
    public class PlayerService : BaseCachedService<PlayerModel, int>
    {


        private readonly IPlayerDAO _playerDAO;
        private readonly PlayerStatsService _playerStatsService;
        private readonly PlayerInventoryService _playerInventoryService;
        private readonly PlayerSpellsService _playerSpellsService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly Dictionary<string, int> _nameToIdCache = new();
        private readonly IAccountService _IAccountService;

        public PlayerService(
            IPlayerDAO playerDAO,
            PlayerLocationService playerLocationService,
            PlayerStatsService playerStatsService,
            PlayerInventoryService playerInventoryService,
            IAccountService IAccountService,
            PlayerSpellsService playerSpellsService
            )
        {
            _playerDAO = playerDAO;
            _playerLocationService = playerLocationService;
            _playerInventoryService = playerInventoryService;
            _playerStatsService = playerStatsService;
            _IAccountService = IAccountService;
            _playerSpellsService = playerSpellsService;
        }

        // ============================= 
        // CRUD BÁSICO 
        // ============================= 

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId, PlayerModuleOptions? options = null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🔍 Buscando jugador por ID: {playerId}");
            var player = await GetOrCreatePlayerInCacheAsync(playerId);
            if (player == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ No se encontró el jugador con ID {playerId}.");
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
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"💾 Guardando jugador: {player.Name} (ID: {player.Id})");
                var existingPlayer = await _playerDAO.GetPlayerByNameAsync(player.Name);
                if (existingPlayer != null && existingPlayer.Id != player.Id)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ Ya existe un jugador con el nombre '{player.Name}'.");
                    return;
                }

                // Guardar el jugador
                await _playerDAO.SavePlayerAsync(player);
                _nameToIdCache[player.Name] = player.Id;
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "✅ Jugador guardado correctamente.");

                // Verificar y guardar los módulos (stats, inventory, spells, location)
                if (player.Stats != null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "📊 Guardando estadísticas del jugador...");
                    await _playerStatsService.SavePlayerStatsAsync(player.Stats);
                }

                if (player.Inventory != null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "🎒 Guardando inventario del jugador...");
                    await _playerInventoryService.SaveInventoryAsync(player.Inventory);
                }

                if (player.PlayerSpells != null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "✨ Guardando hechizos del jugador...");
                    await _playerSpellsService.UpsertSpellAsync(player.PlayerSpells);
                }

                if (player.Location != null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "📍 Guardando ubicación del jugador...");
                    await _playerLocationService.SavePlayerLocationAsync(player.Location);
                }

            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al guardar el jugador: {ex.Message}");
            }
        }


        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🗑️ Eliminando jugador con ID: {playerId}");
                var deleted = await _playerDAO.DeletePlayerAsync(playerId);
                if (deleted)
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "✅ Jugador eliminado correctamente.");
                return deleted;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al eliminar el jugador: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🔍 Verificando existencia del jugador ID: {playerId}");
                return await _playerDAO.PlayerExistsAsync(playerId);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al verificar la existencia del jugador: {ex.Message}");
                return false;
            }
        }
        public async Task<AccountModel?> GetAccountByPlayerIdAsync(int playerId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🔍 Obteniendo cuenta asociada al jugador ID: {playerId}");
            var player = await GetPlayerByIdAsync(playerId);
            if (player == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ No se pudo obtener la cuenta porque el jugador no existe.");
                return null;
            }

            return await _IAccountService.GetOrCreateAccountInCacheAsync(player.AccountId);
        }
        // ============================= 
        // CREACIÓN DE JUGADOR 
        // ============================= 

        public async Task<PlayerModel?> CreatePlayer(Guid accountId, CreatePlayerDTO createPlayerDTO)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🛠️ Creando jugador '{createPlayerDTO.Name}' para cuenta {accountId}");
                bool nameExists = await _playerDAO.PlayerNameExistsAsync(createPlayerDTO.Name);
                if (nameExists)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ Ya existe un jugador con el nombre '{createPlayerDTO.Name}'.");
                    return null;
                }
                var player = await _playerDAO.CreatePlayerAsync(accountId, createPlayerDTO);
                await InitializePlayerModules(player, createPlayerDTO);

                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"✅ Jugador '{player.Name}' creado exitosamente con ID {player.Id}.");
                return player;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al crear el jugador: {ex.Message}");
                return null;
            }
        }

        private async Task InitializePlayerModules(PlayerModel player, CreatePlayerDTO createPlayerDTO)
        {
            var location = await _playerLocationService.CreateDefaultPlayerLocation(player);
            var stats = await _playerStatsService.CreateDefaultPlayerStatsAsync(player.Id, createPlayerDTO);
            var inventory = await _playerInventoryService.CreateDefaultInventory(player);
            var spells = await _playerSpellsService.CreateDefaultSpells(player);

            Console.WriteLine($"Ubicación: {location}");

            player.Location = location;
            player.Stats = stats;
            player.PlayerSpells = spells;
            player.Inventory = inventory; // Si se utiliza más adelante, descomentar esta línea.
        }

        // ============================= 
        // CONSULTAS 
        // ============================= 

        public async Task<List<PlayerModel>> GetAllPlayers()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, "📄 Obteniendo todos los jugadores...");
            return await _playerDAO.GetAllPlayersAsync();
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountId(Guid accountId, PlayerModuleOptions? options = null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📄 Obteniendo jugadores de la cuenta: {accountId}");
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
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"🔍 Buscando jugador por nombre: {name}");
            if (_nameToIdCache.TryGetValue(name, out int cachedId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📦 Jugador encontrado en caché: {cachedId}");
                return await GetOrLoadAsync(cachedId);
            }

            var player = await _playerDAO.GetPlayerByNameAsync(name);
            if (player != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"✅ Jugador '{name}' encontrado en base de datos con ID {player.Id}");
                Set(player.Id, player);
                _nameToIdCache[name] = player.Id;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ No se encontró el jugador '{name}'");
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
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📦 Jugador ID {playerId} recuperado de caché.");
                return cachedPlayer;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📥 Jugador ID {playerId} no estaba en caché. Cargando desde base de datos...");
            var player = await _playerDAO.GetPlayerByIdAsync(playerId);
            if (player != null)
            {
                Set(player.Id, player);
                _nameToIdCache[player.Name] = playerId;
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"✅ Jugador ID {playerId} cargado y cacheado.");
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ No se encontró el jugador con ID {playerId} en la base de datos.");
            }

            return player;
        }

        protected override async Task<PlayerModel?> LoadModelAsync(int playerId)
        {
            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📤 Cargando modelo del jugador ID {playerId}...");
                var player = await _playerDAO.GetPlayerByIdAsync(playerId).ConfigureAwait(false);
                if (player != null)
                {
                    _nameToIdCache[player.Name] = player.Id;
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"✅ Modelo del jugador ID {playerId} cargado.");
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"⚠️ No se encontró el modelo del jugador ID {playerId}.");
                }
                return player;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al cargar el jugador desde LoadModelAsync: {ex.Message}");
                return null;
            }
        }

        private async Task LoadPlayerModulesAsync(PlayerModel player, PlayerModuleOptions options)
        {
            var tasks = new List<Task>();
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"📊 Cargando modulos {player.Name} (ID: {player.Id})...");

            if (options.IncludeStats)
            {
                tasks.Add(LoadModuleWithLogging(_playerStatsService.LoadPlayerStatsInModel(player), "Estadísticas", player));
            }

            if (options.IncludeInventory)
            {
                tasks.Add(LoadModuleWithLogging(_playerInventoryService.LoadPlayerInventoryInModule(player), "Inventario", player));
            }

            if (options.IncludeLocation)
            {
                tasks.Add(LoadModuleWithLogging(_playerLocationService.LoadPlayerLocationInModel(player), "Ubicación", player));
            }

            if (options.IncludeSpells)
            {
                tasks.Add(LoadModuleWithLogging(_playerSpellsService.LoadPlayerSpellsInModel(player), "Hechizos", player));
            }

            await Task.WhenAll(tasks);
        }

        private async Task LoadModuleWithLogging(Task loadTask, string moduleName, PlayerModel player)
        {
            try
            {
                await loadTask;
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"✅ {moduleName} cargado correctamente para el jugador {player.Name}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerService, $"❌ Error al cargar {moduleName} para el jugador {player.Name}: {ex.Message}");
            }
        }
    }
}

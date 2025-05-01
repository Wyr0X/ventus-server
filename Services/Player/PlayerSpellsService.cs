using VentusServer.Domain.Models;

namespace VentusServer.Services
{


    public class PlayerSpellsService : BaseCachedService<PlayerSpellsModel, int> // Cambio de herencia
    {
        private readonly IPlayerSpellsDAO _playerSpellsDAO;

        public PlayerSpellsService(IPlayerSpellsDAO playerSpellsDAO, TimeSpan? ttl = null) : base(ttl) // Constructor con TTL
        {
            _playerSpellsDAO = playerSpellsDAO;
        }

        protected override async Task<PlayerSpellsModel?> LoadModelAsync(int playerId)
        {
            Console.WriteLine($"[PlayerSpellsService] LoadModelAsync: Cargando inventario de hechizos del jugador desde la base de datos. PlayerId: {playerId}");
            return await _playerSpellsDAO.GetByPlayerId(playerId);
        }
        public async Task<PlayerSpellsModel?> GetPlayerSpellsByIdAsync(int id)
        {
            Console.WriteLine($"[PlayerSpellsService] GetPlayerSpellsByIdAsync: Cargando inventario de hechizos con ID: {id}");
            return await _playerSpellsDAO.GetByIdAsync(id);
        }
        public async Task<PlayerSpellsModel> CreateDefaultSpells(PlayerModel playerModel)
        {
            var spellsModel = new PlayerSpellsModel
            {
                PlayerId = playerModel.Id,
                MaxSlots = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Spells = new List<PlayerSpellModel>()
            };

            await _playerSpellsDAO.CreateAsync(spellsModel);
            Set(playerModel.Id, spellsModel); // Agregar al caché
            return spellsModel;
        }

        public async Task<PlayerSpellsModel?> GetPlayerSpellInventoryByPlayerIdAsync(int playerId)
        {
            return await GetOrLoadAsync(playerId);
        }


        public async Task CreateInventoryAsync(PlayerSpellsModel inventoryModel)
        {
            Console.WriteLine($"[PlayerSpellsService] CreateInventoryAsync: Creando inventario de hechizos para el jugador: {inventoryModel.PlayerId}");
            await _playerSpellsDAO.CreateAsync(inventoryModel);
            Set(inventoryModel.PlayerId, inventoryModel); //agregar al cache
        }

        public async Task UpsertInventoryAsync(PlayerSpellsModel inventoryModel)
        {
            Console.WriteLine($"[PlayerSpellsService] UpsertInventoryAsync: Guardando/Actualizando inventario de hechizos para el jugador: {inventoryModel.PlayerId}");
            await _playerSpellsDAO.UpsertAsync(inventoryModel);
            Set(inventoryModel.PlayerId, inventoryModel);
        }

        public async Task DeleteInventoryByPlayerIdAsync(int playerId)
        {
            Console.WriteLine($"[PlayerSpellsService] DeleteInventoryByPlayerIdAsync: Eliminando inventario de hechizos del jugador con ID de jugador: {playerId}");
            await _playerSpellsDAO.DeleteByPlayerId(playerId);
            Invalidate(playerId); // Invalida la entrada de caché
        }

        public async Task AddSpellToInventoryAsync(int playerId, PlayerSpellModel spell)
        {
            var inventory = await GetOrLoadAsync(playerId); // Obtener del cache

            if (inventory == null)
            {
                inventory = new PlayerSpellsModel
                {
                    PlayerId = playerId,
                    Spells = new List<PlayerSpellModel>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    MaxSlots = 10
                };
                await CreateInventoryAsync(inventory);
            }

            var existingSpell = inventory.Spells.Find(s => s.SpellId == spell.SpellId);
            if (existingSpell != null)
            {
                existingSpell.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                inventory.Spells.Add(spell);
            }
            inventory.UpdatedAt = DateTime.UtcNow;
            await UpsertInventoryAsync(inventory);
        }

        public async Task RemoveSpellFromInventoryAsync(int playerId, string spellId)
        {
            var inventory = await GetOrLoadAsync(playerId); //obtener del cache
            if (inventory == null)
            {
                Console.WriteLine($"⚠️ No se encontró el inventario de hechizos para el jugador {playerId}.");
                return;
            }

            var spellToRemove = inventory.Spells.Find(s => s.SpellId == spellId);
            if (spellToRemove != null)
            {
                inventory.Spells.Remove(spellToRemove);
                inventory.UpdatedAt = DateTime.UtcNow;
                await UpsertInventoryAsync(inventory);
                Console.WriteLine($"✅ Hechizo {spellId} eliminado del inventario del jugador {playerId}.");
            }
            else
            {
                Console.WriteLine($"⚠️ El hechizo {spellId} no se encontró en el inventario del jugador {playerId}.");
            }
        }
        public async Task<PlayerSpellsModel?> LoadPlayerSpellsInModel(PlayerModel playerModel)
        {
            var spell = await GetPlayerSpellsByIdAsync(playerModel.Id);

            playerModel.PlayerSpells = spell;
            return spell;
        }
    }
}

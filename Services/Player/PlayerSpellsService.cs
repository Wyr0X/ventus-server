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
            return await _playerSpellsDAO.GetByPlayerId(playerId).ConfigureAwait(false);
        }
        public async Task<PlayerSpellsModel?> GetPlayerSpellsByIdAsync(int id)
        {
            return await _playerSpellsDAO.GetByIdAsync(id).ConfigureAwait(false);
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

            await _playerSpellsDAO.CreateAsync(spellsModel).ConfigureAwait(false);
            Set(playerModel.Id, spellsModel); // Agregar al caché
            return spellsModel;
        }

        public async Task<PlayerSpellsModel?> GetPlayerSpellsByPlayerIdAsync(int playerId)
        {
            return await GetOrLoadAsync(playerId).ConfigureAwait(false);
        }


        public async Task CreateSpellsAsync(PlayerSpellsModel spellsModel)
        {
            await _playerSpellsDAO.CreateAsync(spellsModel).ConfigureAwait(false);
            Set(spellsModel.PlayerId, spellsModel); //agregar al cache
        }

        public async Task UpsertSpellAsync(PlayerSpellsModel spellsModel)
        {
            await _playerSpellsDAO.UpsertAsync(spellsModel).ConfigureAwait(false);
            Set(spellsModel.PlayerId, spellsModel);
        }

        public async Task DeleteSpellsByPlayerIdAsync(int playerId)
        {
            await _playerSpellsDAO.DeleteByPlayerId(playerId).ConfigureAwait(false);
            Invalidate(playerId); // Invalida la entrada de caché
        }

        public async Task AddSpellAsync(int playerId, PlayerSpellModel spell)
        {
            var inventory = await GetOrLoadAsync(playerId).ConfigureAwait(false); // Obtener del cache

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
                await CreateSpellsAsync(inventory).ConfigureAwait(false);
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
            await UpsertSpellAsync(inventory).ConfigureAwait(false);
        }

        public async Task RemoveSpellAsync(int playerId, string spellId)
        {
            var spells = await GetOrLoadAsync(playerId).ConfigureAwait(false); //obtener del cache
            if (spells == null)
            {
                return;
            }

            var spellToRemove = spells.Spells.Find(s => s.SpellId == spellId);
            if (spellToRemove != null)
            {
                spells.Spells.Remove(spellToRemove);
                spells.UpdatedAt = DateTime.UtcNow;
                await UpsertSpellAsync(spells).ConfigureAwait(false);
            }
            else
            {
            }
        }
        public async Task<PlayerSpellsModel?> LoadPlayerSpellsInModel(PlayerModel playerModel)
        {
            var spell = await GetPlayerSpellsByIdAsync(playerModel.Id).ConfigureAwait(false);

            playerModel.PlayerSpells = spell;
            return spell;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DTOs;

namespace VentusServer.Services
{
    public class SpellService : BaseCachedService<SpellModel, string>
    {
        private readonly ISpellDAO _spellDAO;

        public SpellService(ISpellDAO spellDAO, TimeSpan? ttl = null)
            : base(ttl)
        {
            _spellDAO = spellDAO;
        }

        // Cargar el modelo de hechizo desde la fuente externa (base de datos)
        protected override async Task<SpellModel?> LoadModelAsync(string spellId)
        {
            var spell = await _spellDAO.GetSpellByIdAsync(spellId).ConfigureAwait(false);
            return spell;
        }

        public async Task<IEnumerable<SpellModel>> GetAllSpellsAsync()
        {
            var spells = await _spellDAO.GetAllSpellsAsync().ConfigureAwait(false);
            return spells;
        }

        public async Task<SpellModel?> GetSpellByIdAsync(string spellId)
        {
            // Intentar cargar desde la caché
            var spell = await GetOrLoadAsync(spellId).ConfigureAwait(false);
            return spell;
        }

        public async Task CreateSpellAsync(SpellModel spellModel)
        {

            await _spellDAO.CreateSpellAsync(spellModel).ConfigureAwait(false);

            // Después de crear el hechizo, almacenarlo en caché
            Set(spellModel.Id, spellModel);
        }

        public async Task UpdateSpellAsync(SpellModel spellModel)
        {


            await _spellDAO.SaveSpellAsync(spellModel).ConfigureAwait(false);

            // Actualizar el hechizo en caché después de actualizar
            Set(spellModel.Id, spellModel);
        }

        public async Task DeleteSpellAsync(string spellId)
        {
            var exists = await _spellDAO.SpellExistsAsync(spellId).ConfigureAwait(false);
            if (!exists)
            {
                throw new Exception($"Spell with ID {spellId} does not exist.");
            }

            await _spellDAO.DeleteSpellAsync(spellId).ConfigureAwait(false);

            // Invalidar el caché de este hechizo después de eliminar
            Invalidate(spellId);
        }
    }
}

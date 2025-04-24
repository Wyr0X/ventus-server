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
            var spell = await _spellDAO.GetSpellByIdAsync(spellId);
            return spell;
        }

        public async Task<IEnumerable<SpellModel>> GetAllSpellsAsync()
        {
            var spells = await _spellDAO.GetAllSpellsAsync();
            return spells;
        }

        public async Task<SpellModel?> GetSpellByIdAsync(string spellId)
        {
            // Intentar cargar desde la caché
            var spell = await GetOrLoadAsync(spellId);
            return spell;
        }

        public async Task CreateSpellAsync(SpellModel spellModel)
        {

            await _spellDAO.CreateSpellAsync(spellModel);

            // Después de crear el hechizo, almacenarlo en caché
            Set(spellModel.Id, spellModel);
        }

        public async Task UpdateSpellAsync(SpellModel spellModel)
        {


            await _spellDAO.SaveSpellAsync(spellModel);

            // Actualizar el hechizo en caché después de actualizar
            Set(spellModel.Id, spellModel);
        }

        public async Task DeleteSpellAsync(string spellId)
        {
            var exists = await _spellDAO.SpellExistsAsync(spellId);
            if (!exists)
            {
                throw new Exception($"Spell with ID {spellId} does not exist.");
            }

            await _spellDAO.DeleteSpellAsync(spellId);

            // Invalidar el caché de este hechizo después de eliminar
            Invalidate(spellId);
        }
    }
}

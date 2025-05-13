
namespace VentusServer.DataAccess.Interfaces
{
    public interface ISpellDAO
    {
        Task InitializeTableAsync();
        Task<SpellModel?> GetSpellByIdAsync(string spellId);
        Task SaveSpellAsync(SpellModel spell);
        Task CreateSpellAsync(SpellModel spell);
        Task DeleteSpellAsync(string spellId);
        Task<bool> SpellExistsAsync(string spellId);
        Task<IEnumerable<SpellModel>> GetAllSpellsAsync();
    }
}

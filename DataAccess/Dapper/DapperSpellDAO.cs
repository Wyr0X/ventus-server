using Dapper;
using Game.Models;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperSpellDAO : BaseDAO, ISpellDAO
    {
        public DapperSpellDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task InitializeTableAsync()
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.CreateTableQuery);
        }

        public async Task<SpellModel?> GetSpellByIdAsync(string spellId)
        {
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync(SpellQueries.SelectById, new { Id = spellId });

            if (result == null) return null;

            return SpellMapper.Map(result);
        }

        public async Task SaveSpellAsync(SpellModel spell)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.Update,
                SpellMapper.ToDbParameters(spell));
        }

        public async Task CreateSpellAsync(SpellModel spell)
        {
            try
            {
                Console.WriteLine("Llega 2");
                using var connection = GetConnection();

                await connection.ExecuteAsync(SpellQueries.Insert, SpellMapper.ToDbParameters(spell));
            }
            catch (Exception ex)
            {
                // Aquí puedes agregar detalles de la excepción
                // Puedes incluir el mensaje de la excepción, la pila de llamadas y los parámetros del spell
                var errorMessage = $"Error al crear el hechizo. " +
                                   $"Nombre: {spell.Name}, " +
                                   $"ID: {spell.Id}, " +
                                   $"Nivel requerido: {spell.RequiredLevel}, " +
                                   $"Clase requerida: {spell.RequiredClass}, " +
                                   $"Excepción: {ex.Message}, " +
                                   $"Stack Trace: {ex.StackTrace}";

                // Aquí se lanza la excepción nuevamente o se puede registrar el error en un archivo de logs
                throw new ApplicationException(errorMessage, ex);
            }
        }

        public async Task DeleteSpellAsync(string spellId)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.DeleteById, new { Id = spellId });
        }

        public async Task<bool> SpellExistsAsync(string spellId)
        {
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync<bool>(SpellQueries.ExistsById, new { Id = spellId });
            return result;
        }
        public async Task<IEnumerable<SpellModel>> GetAllSpellsAsync()
        {
            using var connection = GetConnection();

            var result = await connection.QueryAsync(SpellQueries.SelectAll);
            var spells = new List<SpellModel>();

            foreach (var spell in result)
            {
                try
                {
                    Console.WriteLine($"[SpellController] Procesando spell ID: {spell.id}, effects: {spell.effects}");
                    var mapped = SpellMapper.Map(spell);
                    spells.Add(mapped);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SpellController] Error al procesar spell ID: {spell.id}. Error: {ex.Message}");
                }
            }

            return spells;
        }

    }
}

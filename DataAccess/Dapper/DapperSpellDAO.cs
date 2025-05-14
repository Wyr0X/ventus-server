using Dapper;
using Game.Models;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers;
using static LoggerUtil;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperSpellDAO : BaseDAO, ISpellDAO
    {
        public DapperSpellDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task InitializeTableAsync()
        {
            Log(LogTag.DapperSpellDAO, "Inicializando tabla de hechizos...");
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.CreateTableQuery);
            Log(LogTag.DapperSpellDAO, "Tabla de hechizos inicializada.");
        }

        public async Task<SpellModel?> GetSpellByIdAsync(string spellId)
        {
            Log(LogTag.DapperSpellDAO, $"Buscando hechizo por ID: {spellId}");
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync(SpellQueries.SelectById, new { Id = spellId });

            if (result == null)
            {
                Log(LogTag.DapperSpellDAO, $"Hechizo con ID {spellId} no encontrado.");
                return null;
            }

            Log(LogTag.DapperSpellDAO, $"Hechizo con ID {spellId} encontrado.");
            return SpellMapper.ToModel(result);
        }

        public async Task SaveSpellAsync(SpellModel spell)
        {
            Log(LogTag.DapperSpellDAO, $"Actualizando hechizo ID: {spell.Id}, Nombre: {spell.Name}");
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.Update, SpellMapper.ToDBEntity(spell));
            Log(LogTag.DapperSpellDAO, $"Hechizo actualizado: {spell.Id}");
        }

        public async Task CreateSpellAsync(SpellModel spell)
        {
            try
            {
                var dbEntity = SpellMapper.ToDBEntity(spell);
                var sql = SpellQueries.Insert; // O el query que uses para crear spells

                // Loguea el SQL y los parámetros
                Console.WriteLine(spell.CastType);

                Console.WriteLine("----- SQL QUE SE VA A EJECUTAR -----");
                Console.WriteLine(sql);
                Console.WriteLine("----- PARÁMETROS -----");
                foreach (var prop in dbEntity.GetType().GetProperties())
                {
                    var value = prop.GetValue(dbEntity);
                    Console.WriteLine($"{prop.Name}: {value}");
                }
                Console.WriteLine("------------------------------------");

                using var connection = GetConnection();
                await connection.ExecuteAsync(sql, dbEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DapperSpellDAO] Error al crear el hechizo. Nombre: {spell.Name}");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task DeleteSpellAsync(string spellId)
        {
            Log(LogTag.DapperSpellDAO, $"Eliminando hechizo con ID: {spellId}");
            using var connection = GetConnection();
            await connection.ExecuteAsync(SpellQueries.DeleteById, new { Id = spellId });
            Log(LogTag.DapperSpellDAO, $"Hechizo eliminado: {spellId}");
        }

        public async Task<bool> SpellExistsAsync(string spellId)
        {
            Log(LogTag.DapperSpellDAO, $"Verificando existencia del hechizo ID: {spellId}");
            using var connection = GetConnection();
            var result = await connection.QuerySingleOrDefaultAsync<bool>(SpellQueries.ExistsById, new { Id = spellId });
            Log(LogTag.DapperSpellDAO, $"Hechizo {(result ? "existe" : "no existe")}: {spellId}");
            return result;
        }

        public async Task<IEnumerable<SpellModel>> GetAllSpellsAsync()
        {
            Log(LogTag.DapperSpellDAO, "Obteniendo todos los hechizos...");

            using var connection = GetConnection();

            // Ejecutamos la consulta y obtenemos los resultados
            var result = await connection.QueryAsync<SpellDBEntity>(SpellQueries.SelectAll);

            var spells = new List<SpellModel>();

            // Mapeamos cada hechizo
            foreach (var spell in result)
            {
                try
                {

                    var mapped = SpellMapper.ToModel(spell);
                    spells.Add(mapped);
                }
                catch (Exception ex)
                {
                    Log(LogTag.DapperSpellDAO, $"Error al mapear hechizo ID {spell.Id}: {ex.Message}", isError: true);
                }
            }

            // Log del conteo final de hechizos
            Log(LogTag.DapperSpellDAO, $"Total de hechizos obtenidos: {spells.Count}");

            // Devolvemos la lista de hechizos mapeados
            return spells;
        }

    }
}

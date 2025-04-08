using System;
using System.Threading.Tasks;
using Npgsql;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Seeders;

namespace VentusServer.DataAccess.Postgres
{
    public class DatabaseInitializer
    {
        private readonly PostgresDbService _postgresDbService;

        private IRoleDAO _roleDAO;
        public DatabaseInitializer(PostgresDbService postgresDbService, IRoleDAO roleDAO)
        {
            _postgresDbService = postgresDbService;
            _roleDAO = roleDAO;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                await InitializeRolesAsync();
                await InitializeAccountsAsync();
                await InitializePlayersAsync();
                await InitializeWorldsAsync();
                await InitializeMapsAsync();
                await InitializePlayerLocationsAsync();
                await InitializePlayerStatsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error durante la inicialización de la base de datos: {ex.Message}");
            }
        }

        private async Task InitializeAccountsAsync()
        {
            try
            {


                await _postgresDbService.ExecuteQueryAsync(AccountQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'accounts' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'accounts': {ex.Message}");
            }
        }

        private async Task InitializePlayersAsync()
        {
            try
            {

                await _postgresDbService.ExecuteQueryAsync(PlayerQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'players' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'players': {ex.Message}");
            }
        }

        public async Task InitializePlayerLocationsAsync()
        {
            try
            {

                await _postgresDbService.ExecuteQueryAsync(PlayerLocationQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'player_locations' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'player_locations': {ex.Message}");
            }
        }

        public async Task InitializeWorldsAsync()
        {
            try
            {


                await _postgresDbService.ExecuteQueryAsync(WorldQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'worlds' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'worlds': {ex.Message}");
            }
        }

        public async Task InitializeMapsAsync()
        {
            try
            {

                await _postgresDbService.ExecuteQueryAsync(MapQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'maps' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'maps': {ex.Message}");
            }
        }
        private async Task InitializePlayerStatsAsync()
        {
            try
            {

                await _postgresDbService.ExecuteQueryAsync(PlayerStatsQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'player_stats' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'player_stats': {ex.Message}");
            }
        }
        private async Task InitializeRolesAsync()
        {
            try
            {

                await _postgresDbService.ExecuteQueryAsync(RoleQueries.CreateTableQuery);
                Console.WriteLine("✅ Tabla 'player_roles' creada correctamente (si no existía).");
                await RoleSeeder.SeedRolesAsync(_roleDAO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'player_roles': {ex.Message}");
            }
        }
    }

}

// DROP TABLE IF EXISTS player_locations CASCADE;
// DROP TABLE IF EXISTS player_stats CASCADE;
// DROP TABLE IF EXISTS players CASCADE;
// DROP TABLE IF EXISTS maps CASCADE;
// DROP TABLE IF EXISTS worlds CASCADE;
// DROP TABLE IF EXISTS accounts CASCADE;

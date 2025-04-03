using System;
using System.Threading.Tasks;
using Npgsql;

namespace VentusServer.DataAccess.Postgres
{
    public class DatabaseInitializer
    {
        private readonly PostgresDbService _postgresDbService;

        public DatabaseInitializer(PostgresDbService postgresDbService)
        {
            _postgresDbService = postgresDbService;
        }

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                await InitializeAccountsAsync();
                await InitializePlayersAsync();
                await InitializePlayerLocationsAsync();
                await InitializeWorldsAsync();
                await InitializeMapsAsync();
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
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS accounts (
                        account_id UUID PRIMARY KEY,
                        email VARCHAR(255) UNIQUE NOT NULL,
                        password VARCHAR(255) NOT NULL,
                        is_deleted BOOLEAN DEFAULT FALSE,
                        is_banned BOOLEAN DEFAULT FALSE,
                        credits INT DEFAULT 0,
                        last_ip VARCHAR(45),
                        last_login TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        name VARCHAR(100) NOT NULL
                    );
                ";

                await _postgresDbService.ExecuteQueryAsync(createTableQuery);
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
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS players (
                        id SERIAL PRIMARY KEY,
                        account_id UUID NOT NULL REFERENCES accounts(account_id) ON DELETE CASCADE,
                        name VARCHAR(100) NOT NULL,
                        gender VARCHAR(10) NOT NULL,
                        race VARCHAR(50) NOT NULL,
                        level INT DEFAULT 1,
                        class VARCHAR(50) NOT NULL,
                        created_at TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        last_login TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                        status VARCHAR(50) DEFAULT 'active'
                    );
                ";

                await _postgresDbService.ExecuteQueryAsync(createTableQuery);
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
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS player_locations (
                        player_id INT PRIMARY KEY REFERENCES players(id) ON DELETE CASCADE,
                        world_id INT NOT NULL REFERENCES worlds(id),
                        map_id INT NOT NULL REFERENCES maps(id),
                        pos_x INT NOT NULL,
                        pos_y INT NOT NULL,
                        direction VARCHAR(10) NOT NULL
                    );
                ";

                await _postgresDbService.ExecuteQueryAsync(createTableQuery);
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
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS worlds (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        description TEXT,
                        max_maps INT NOT NULL,
                        max_players INT NOT NULL,
                        level_requirements INT NOT NULL
                    );
                ";

                await _postgresDbService.ExecuteQueryAsync(createTableQuery);
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
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS maps (
                        id SERIAL PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        min_level INT NOT NULL,
                        max_players INT NOT NULL,
                        world_id INT NOT NULL REFERENCES worlds(id) ON DELETE CASCADE
                    );
                ";

                await _postgresDbService.ExecuteQueryAsync(createTableQuery);
                Console.WriteLine("✅ Tabla 'maps' creada correctamente (si no existía).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al crear la tabla 'maps': {ex.Message}");
            }
        }
    }
}
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
                // Inicializar la tabla 'accounts'
                await InitializeAccountsAsync();
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
                        user_id VARCHAR(50) PRIMARY KEY, -- Almacena el UID de Firebase como cadena
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
    }
}

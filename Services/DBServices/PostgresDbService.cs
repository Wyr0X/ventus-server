using System;
using System.IO;
using System.Threading.Tasks;
using dotenv.net;
using Npgsql;

namespace VentusServer
{
    public class PostgresDbService
    {
        private readonly string _connectionString;

        public PostgresDbService()
        {
            // Cargar las variables de entorno desde un archivo .env
            DotEnv.Load();

            // Obtener las credenciales desde las variables de entorno
            string? host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            string? username = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string? password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            string? dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");

            // Verificar que las variables de entorno no sean nulas o vacías
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dbName))
            {
                throw new InvalidOperationException("No se encontraron las credenciales necesarias en el archivo .env");
            }

            // Construir la cadena de conexión
            _connectionString = $"Host={host};Username={username};Password={password};Database={dbName}";
        }

        // Método para verificar la conexión a la base de datos
        public async Task<bool> CheckConnectionAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();  // Intenta abrir la conexión
                    return true;  // Si la conexión es exitosa, devuelve true
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al intentar conectar a la base de datos: {ex.Message}");
                return false;  // Si ocurre algún error, devuelve false
            }
        }

        public async Task ExecuteQueryAsync(string query)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<T?> ExecuteScalarQueryAsync<T>(string query)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return result == DBNull.Value ? default : (T?)result;
                }
            }
        }

        public async Task<NpgsqlDataReader> ExecuteReaderQueryAsync(string query)
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using (var command = new NpgsqlCommand(query, connection))
            {
                return await command.ExecuteReaderAsync();
            }
        }

        // Función para crear tablas
        public async Task CreateTableAsync(string tableName, string tableDefinition)
        {
            // Definir la consulta CREATE TABLE
            string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} ({tableDefinition});";

            // Ejecutar la consulta para crear la tabla
            await ExecuteQueryAsync(createTableQuery);
        }
    }
}

using dotenv.net;

public static class EnvValidator
{
    public static (string FirebasePath, string PostgresConnectionString) ValidateAndBuild()
    {
        DotEnv.Load();

        string credentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? string.Empty;
        if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
        {
            throw new Exception("No se encontró el archivo de credenciales de Firebase.");
        }

        string host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        string username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password";
        string dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "ventus";

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dbName))
        {
            throw new Exception("Faltan variables de entorno para la base de datos.");
        }

        string postgresConnectionString = $"Host={host};Username={username};Password={password};Database={dbName};Include Error Detail=true";

        return (credentialsPath, postgresConnectionString);
    }
}

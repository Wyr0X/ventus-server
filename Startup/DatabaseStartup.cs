using Microsoft.Extensions.DependencyInjection;
using VentusServer;
using VentusServer.DataAccess.Postgres;

public static class DatabaseStartup
{
    public static async Task<bool> InitDatabase(IServiceProvider serviceProvider)
    {
        var dbService = serviceProvider.GetRequiredService<PostgresDbService>();
        bool connected = await dbService.CheckConnectionAsync();

        if (!connected)
        {
            Log.Log(Log.LogTag.DB, "No se pudo conectar a la base de datos.");
            return false;
        }

        var initializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeDatabaseAsync();

        Log.Log(Log.LogTag.DB, "Conexión e inicialización de base de datos exitosa.");
        return true;
    }
}

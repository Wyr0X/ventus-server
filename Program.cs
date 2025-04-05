using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using VentusServer;

DotEnv.Load();
LoggerUtil.Log("ENV", "Variables de entorno cargadas.", ConsoleColor.Cyan);

(string credentialsPath, string postgresConnectionString) = EnvValidator.ValidateAndBuild();
LoggerUtil.Log("ENV", "Credenciales y cadena de conexión construidas correctamente.", ConsoleColor.Cyan);

var serviceModule = ServiceProviderModule.Build(credentialsPath, postgresConnectionString);
var serviceProvider = serviceModule.Provider;
LoggerUtil.Log("DI", "Contenedor de dependencias configurado.", ConsoleColor.Blue);

try
{
    LoggerUtil.Log("DB", "Iniciando verificación e inicialización de la base de datos...", ConsoleColor.Yellow);
    bool dbReady = await DatabaseStartup.InitDatabase(serviceProvider);
    if (!dbReady)
    {
        LoggerUtil.Log("DB", "Inicialización de base de datos fallida. Terminando ejecución.", ConsoleColor.Red);
        return;
    }

    LoggerUtil.Log("GAME", "Iniciando componentes del juego...", ConsoleColor.Yellow);
    GameStartup.StartGameComponents(serviceProvider);

    LoggerUtil.Log("HTTP", "Inicializando servidor HTTP...", ConsoleColor.Yellow);
    var webHost = HttpServerBuilder.BuildHost(serviceModule.Services);

    LoggerUtil.Log("HTTP", "Servidor HTTP corriendo en http://localhost:5000", ConsoleColor.Green);
    await webHost.RunAsync();
}
catch (Exception ex)
{
    LoggerUtil.Log("ERROR", $"Error durante la inicialización: {ex.Message}", ConsoleColor.Red);
    LoggerUtil.Log("ERROR", $"StackTrace: {ex.StackTrace}", ConsoleColor.DarkRed);
}

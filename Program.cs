using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VentusServer;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

try
{
    StartupLogger.Log("⏳ Cargando variables de entorno...");
    DotEnv.Load();
    StartupLogger.Log("✅ Variables de entorno cargadas.");

    StartupLogger.Log("⏳ Validando y construyendo conexión...");
    (string credentialsPath, string postgresConnectionString) = EnvValidator.ValidateAndBuild();
    StartupLogger.Log("✅ Conexión y credenciales listas.");

    StartupLogger.Log("⏳ Configurando contenedor de dependencias...");
    var serviceModule = ServiceProviderModule.Build(credentialsPath, postgresConnectionString);
    var serviceProvider = serviceModule.Provider;
    StartupLogger.Log("✅ Contenedor configurado.");

    StartupLogger.Log("⏳ Verificando e inicializando la base de datos...");
    bool dbReady = await DatabaseStartup.InitDatabase(serviceProvider);
    if (!dbReady)
    {
        StartupLogger.Log("❌ Inicialización de base de datos fallida. Terminando ejecución.");
        return;
    }

    StartupLogger.Log("✅ Base de datos inicializada correctamente.");

    using var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    StartupLogger.Log("⏳ Iniciando componentes del juego...");
    var webSocketServerController = serviceProvider.GetRequiredService<WebSocketServerController>();
    var gameEngine = serviceProvider.GetRequiredService<GameServer>();

    var wsServerTask = Task.Run(() =>
    {
        StartupLogger.Log("🌐 WebSocketServer iniciando...");
        return webSocketServerController.StartServerAsync(cancellationToken);
    }, cancellationToken);

    var wsLoopTask = Task.Run(() =>
    {
        StartupLogger.Log("🔄 WebSocketServer loop iniciando...");
        return webSocketServerController.StartLoop(cancellationToken);
    }, cancellationToken);

    var gameEngineTask = Task.Run(() =>
    {
        StartupLogger.Log("🧠 GameEngine iniciando...");
        return gameEngine.Run(cancellationToken);
    }, cancellationToken);

    StartupLogger.Log("🌍 Inicializando servidor HTTP...");
    var webHost = HttpServerBuilder.BuildHost(serviceModule.Services);
    StartupLogger.Log("✅ Servidor HTTP corriendo en http://localhost:5000");

    await webHost.RunAsync(cancellationToken);

    await Task.WhenAll(wsServerTask, wsLoopTask, gameEngineTask);
}
catch (Exception ex)
{
    StartupLogger.Log($"❌ Error durante la inicialización: {ex.Message}");
    StartupLogger.Log($"StackTrace: {ex.StackTrace}");
}

static class StartupLogger
{
    private static readonly string LogFile = "startup.log";

    public static void Log(string message)
    {
        string fullMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
        Console.WriteLine(fullMessage);
        File.AppendAllText(LogFile, fullMessage + Environment.NewLine);
    }
}

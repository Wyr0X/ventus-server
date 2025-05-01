using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VentusServer;

DotEnv.Load();
LoggerUtil.Log(LoggerUtil.LogTag.Init, "Variables de entorno cargadas.");

(string credentialsPath, string postgresConnectionString) = EnvValidator.ValidateAndBuild();
LoggerUtil.Log(LoggerUtil.LogTag.Init, "Credenciales y cadena de conexión construidas correctamente.");

var serviceModule = ServiceProviderModule.Build(credentialsPath, postgresConnectionString);
var serviceProvider = serviceModule.Provider;
LoggerUtil.Log(LoggerUtil.LogTag.Init, "Contenedor de dependencias configurado.");

try
{
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "Iniciando verificación e inicialización de la base de datos...");
    bool dbReady = await DatabaseStartup.InitDatabase(serviceProvider);
    if (!dbReady)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.Init, "Inicialización de base de datos fallida. Terminando ejecución.");
        return;
    }

    using var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "Base de datos inicializada correctamente.");
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "Iniciando componentes del juego...");

    var webSocketServerController = serviceProvider.GetRequiredService<WebSocketServerController>();
    var gameEngine = serviceProvider.GetRequiredService<GameServer>();

    var wsServerTask = Task.Run(
        () => webSocketServerController.StartServerAsync(cancellationToken),
        cancellationToken
    );
    var wsLoopTask = Task.Run(
        () => webSocketServerController.StartLoop(cancellationToken),
        cancellationToken
    );
    var gameEngineTask = Task.Run(() => gameEngine.Run(cancellationToken), cancellationToken);

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "Inicializando servidor HTTP...");
    var webHost = HttpServerBuilder.BuildHost(serviceModule.Services);

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "Servidor HTTP corriendo en http://localhost:5000");

    // 🔹 Ejecutar servidor HTTP y esperar a cancelación
    await webHost.RunAsync(cancellationToken);

    // 🔹 Esperar a que terminen tareas (opcional, puedes usar WhenAll)
    await Task.WhenAll(wsServerTask, wsLoopTask, gameEngineTask);
}
catch (Exception ex)
{
    LoggerUtil.Log(LoggerUtil.LogTag.Init, $"Error durante la inicialización: {ex.Message}");
    LoggerUtil.Log(LoggerUtil.LogTag.Init, $"StackTrace: {ex.StackTrace}");
}

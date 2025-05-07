using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VentusServer;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Game.Server;

try
{
    LoggerUtil.EnableTag(LoggerUtil.LogTag.Init);

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "⏳ Cargando variables de entorno...");
    DotEnv.Load();
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "✅ Variables de entorno cargadas.");

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "⏳ Validando y construyendo conexión...");
    (string credentialsPath, string postgresConnectionString) = EnvValidator.ValidateAndBuild();
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "✅ Conexión y credenciales listas.");

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "⏳ Configurando contenedor de dependencias...");
    var serviceModule = ServiceProviderModule.Build(credentialsPath, postgresConnectionString);
    var serviceProvider = serviceModule.Provider;
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "✅ Contenedor configurado.");

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "⏳ Verificando e inicializando la base de datos...");
    bool dbReady = await DatabaseStartup.InitDatabase(serviceProvider);
    if (!dbReady)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.Init, "❌ Inicialización de base de datos fallida. Terminando ejecución.", isError: true);
        return;
    }

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "✅ Base de datos inicializada correctamente.");

    using var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "⏳ Iniciando componentes del juego...");
    var webSocketServerController = serviceProvider.GetRequiredService<WebSocketServerController>();
    var gameEngine = serviceProvider.GetRequiredService<GameServer>();

    var wsServerTask = Task.Run(() =>
    {
        LoggerUtil.Log(LoggerUtil.LogTag.Init, "🌐 WebSocketServer iniciando...");
        return webSocketServerController.StartServerAsync(cancellationToken);
    }, cancellationToken);

    var wsLoopTask = Task.Run(() =>
    {
        LoggerUtil.Log(LoggerUtil.LogTag.Init, "🔄 WebSocketServer loop iniciando...");
        webSocketServerController.StartLoop(cancellationToken);
        return Task.CompletedTask;
    }, cancellationToken);

    var gameEngineTask = Task.Run(() =>
    {
        LoggerUtil.Log(LoggerUtil.LogTag.Init, "🧠 GameEngine iniciando...");
        return gameEngine.RunAsync(cancellationToken);
    }, cancellationToken);

    LoggerUtil.Log(LoggerUtil.LogTag.Init, "🌍 Inicializando servidor HTTP...");
    var webHost = HttpServerBuilder.BuildHost(serviceModule.Services);
    LoggerUtil.Log(LoggerUtil.LogTag.Init, "✅ Servidor HTTP corriendo en http://localhost:5000");

    await webHost.RunAsync(cancellationToken);

    await Task.WhenAll(wsServerTask, wsLoopTask, gameEngineTask);
}
catch (Exception ex)
{
    LoggerUtil.Log(LoggerUtil.LogTag.Init, $"❌ Error durante la inicialización: {ex.Message}", isError: true);
    LoggerUtil.Log(LoggerUtil.LogTag.Init, $"StackTrace: {ex.StackTrace}", isError: true);
}

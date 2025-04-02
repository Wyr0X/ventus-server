using dotenv.net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;
using VentusServer;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Postgres;
using VentusServer.Controllers;
using VentusServer.Services;
using Game.Models;

DotEnv.Load();

// Verificar las credenciales de Firebase
string credentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? string.Empty;
if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
{
    Console.WriteLine("❌ No se encontró el archivo de credenciales de Firebase.");
    return;
}

// Obtener las credenciales de PostgreSQL desde las variables de entorno
string host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
string username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password";
string dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "ventus";

if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dbName))
{
    Console.WriteLine("❌ No se encontraron las credenciales necesarias de PostgreSQL.");
    return;
}

// Construir la cadena de conexión de PostgreSQL
string postgresConnectionString = $"Host={host};Username={username};Password={password};Database={dbName}";

// Configuración de las variables JWT desde el archivo de entorno (o se puede usar appsettings.json)
string secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "your-secret-key";
string issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "your-issuer";
string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "your-audience";

// Configurar el contenedor de servicios de Dependency Injection
var serviceProvider = new ServiceCollection()
    .AddSingleton<PostgresDbService>(sp => new PostgresDbService())
    .AddSingleton<FirebaseService>(sp => new FirebaseService(credentialsPath))
    .AddSingleton<FirestoreService>(sp => new FirestoreService(sp.GetRequiredService<FirebaseService>()))
    .AddScoped<IAccountDAO, PostgresAccountDAO>(sp => new PostgresAccountDAO(postgresConnectionString))
    .AddScoped< PostgresPlayerDAO>(sp => new PostgresPlayerDAO(postgresConnectionString))
    .AddScoped< PostgresWorldDAO>(sp => new PostgresWorldDAO(postgresConnectionString))
    .AddScoped<PostgresMapDAO>(sp =>
                    new PostgresMapDAO(postgresConnectionString, sp.GetRequiredService<PostgresWorldDAO>()))
    .AddScoped< PostgresPlayerLocationDAO>(sp => new PostgresPlayerLocationDAO(postgresConnectionString, sp.GetRequiredService<PostgresWorldDAO>()
    , sp.GetRequiredService<PostgresMapDAO>(), sp.GetRequiredService<PostgresPlayerDAO>()))
   .AddSingleton<MessageSender>()


    .AddSingleton(provider => new Lazy<MessageSender>(provider.GetRequiredService<MessageSender>))

    .AddSingleton<GameEngine>()

    .AddSingleton<SessionManager>()
    .AddSingleton<SessionHandler>()

    .AddSingleton<PostgresDbService>()
    .AddSingleton<DatabaseInitializer>()
    .AddSingleton<ConcurrentDictionary<string, WebSocket>>()
    .AddSingleton<AuthHandler>()
    .AddSingleton<MessageHandler>()

    .AddScoped<AuthController>()

    .AddSingleton<PlayerModel>()
    .AddSingleton<PlayerLocation>()
    .AddSingleton<MapModel>()
    .AddSingleton<WorldModel>()




    .AddSingleton<WorldService>()
    .AddSingleton<MapService>()
    .AddSingleton<PlayerService>()
    .AddSingleton<PlayerLocationService>()

    
    .AddSingleton<ResponseService>()  // Registrar el ResponseService
    .AddSingleton<WebSocketServerController>()
    .AddSingleton<JWTService>(sp => new JWTService(secretKey, issuer, audience)) // Registrar JWTService
    .BuildServiceProvider();

try
{
    // Obtener las instancias de servicios desde el contenedor
    var postgresDbService = serviceProvider.GetRequiredService<PostgresDbService>();

    // Verificar la conexión antes de continuar con la inicialización
    bool isConnected = await postgresDbService.CheckConnectionAsync();

    if (!isConnected)
    {
        Console.WriteLine("❌ No se pudo conectar a la base de datos.");
        return;  // Si no se conecta, salimos del programa
    }

    // Si la conexión fue exitosa, inicializamos la base de datos y el servidor
    var databaseInitializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();

    Console.WriteLine("✔️ Conexión a la base de datos exitosa.");

    // Obtener las instancias de otros servicios y arrancar el servidor WebSocket
    var webSocketServerController = serviceProvider.GetRequiredService<WebSocketServerController>();
    var webSocketServerTask = webSocketServerController.StartServerAsync(); // Hacer esto asíncrono pero no bloqueante
    var worldService = serviceProvider.GetRequiredService<WorldService>();
    var webSocketQueueTask = Task.Run(() => webSocketServerController.StartLoop());


    // Iniciar el servidor web (Kestrel)
    var webHost = WebHost.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddSingleton(serviceProvider); // Usar el serviceProvider configurado
        })
        .UseStartup<Startup>() // Usar la clase Startup para configurar el servidor web
        .Build();

    // Iniciar ambos servidores (web y WebSocket)
         await Task.WhenAny(webHost.RunAsync(), webSocketServerTask);  // Ejecutar ambos simultáneamente

}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error durante la inicialización: {ex.Message}");
        Console.WriteLine($"📌 StackTrace: {ex.StackTrace}");

}

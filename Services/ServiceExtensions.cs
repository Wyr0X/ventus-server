using Game.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using VentusServer;
using VentusServer.Controllers;
using VentusServer.DataAccess.Postgres;
using VentusServer.Services;

public static class ServiceExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, string postgresConnectionString, string credentialsPath)
    {
        services.AddSingleton<PostgresDbService>();
        services.AddSingleton<FirebaseService>(sp => new FirebaseService(credentialsPath));
        services.AddSingleton<DatabaseInitializer>();
        services.AddSingleton<WorldService>();
        services.AddSingleton<MapService>();
        services.AddSingleton<PlayerService>();
        services.AddSingleton<PlayerLocationService>();
    }

    public static void AddDAOServices(this IServiceCollection services, string postgresConnectionString)
    {
        services.AddScoped<IAccountDAO, PostgresAccountDAO>(sp => new PostgresAccountDAO(postgresConnectionString));
        services.AddScoped<PostgresPlayerDAO>(sp => new PostgresPlayerDAO(postgresConnectionString));
        services.AddScoped<PostgresWorldDAO>(sp => new PostgresWorldDAO(postgresConnectionString));
        services.AddScoped<PostgresMapDAO>(sp => new PostgresMapDAO(
            postgresConnectionString,
            sp.GetRequiredService<PostgresWorldDAO>()
        ));
        services.AddScoped<PostgresPlayerLocationDAO>(sp => new PostgresPlayerLocationDAO(
            postgresConnectionString,
            sp.GetRequiredService<PostgresWorldDAO>(),
            sp.GetRequiredService<PostgresMapDAO>(),
            sp.GetRequiredService<PostgresPlayerDAO>()
        ));
    }

    public static void AddHandlers(this IServiceCollection services)
    {
        services.AddSingleton<AuthHandler>();
        services.AddSingleton<MessageHandler>();
        services.AddSingleton<SessionManager>();
    }

    public static void AddWebSocketSupport(this IServiceCollection services)
    {
        services.AddSingleton<WebSocketServerController>();

    }

    public static void AddControllers(this IServiceCollection services)
    {
        services.AddScoped<AuthController>();

        services.AddSingleton<GameEngine>();
    }

    public static void AddGameModels(this IServiceCollection services)
    {
        services.AddSingleton<PlayerModel>();
        services.AddSingleton<PlayerLocation>();
        services.AddSingleton<MapModel>();
        services.AddSingleton<WorldModel>();
    }

    public static void AddUtilityServices(this IServiceCollection services)
    {
        services.AddSingleton<ResponseService>();
    }
}

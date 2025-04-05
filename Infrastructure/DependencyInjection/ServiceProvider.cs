using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using VentusServer.Controllers;
using VentusServer.DataAccess.Postgres;
using VentusServer.Services;
using VentusServer.DataAccess;
using Game.Models;

namespace VentusServer
{
    public static class ServiceProviderModule
    {
        public static ServiceProviderContainer Build(string firebaseCredentialsPath, string postgresConnectionString)
        {
            var services = new ServiceCollection();

            RegisterInfrastructure(services, firebaseCredentialsPath);
            RegisterDAOs(services, postgresConnectionString);
            RegisterHandlers(services);
            RegisterManagers(services);
            RegisterServices(services);
            RegisterModels(services);
            RegisterControllers(services);

            var provider = services.BuildServiceProvider();
            return new ServiceProviderContainer(services, provider);
        }


        private static void RegisterInfrastructure(IServiceCollection services, string firebaseCredentialsPath)
        {
            services
                .AddSingleton<PostgresDbService>()
                .AddSingleton<FirebaseService>(sp => new FirebaseService(firebaseCredentialsPath))
                .AddSingleton<JwtService>()
                .AddSingleton<GameEngine>()
                .AddSingleton<DatabaseInitializer>()
                .AddSingleton<ConcurrentDictionary<string, WebSocket>>()
                .AddSingleton<MessageSender>()
                .AddSingleton(provider => new Lazy<MessageSender>(provider.GetRequiredService<MessageSender>));
        }

        private static void RegisterDAOs(IServiceCollection services, string connectionString)
        {
            services
                .AddScoped<PostgresAccountDAO>(_ => new PostgresAccountDAO(connectionString))
                .AddScoped<PostgresPlayerDAO>(_ => new PostgresPlayerDAO(connectionString))
                .AddScoped<PostgresWorldDAO>(_ => new PostgresWorldDAO(connectionString))
                .AddScoped<PostgresMapDAO>(sp =>
                    new PostgresMapDAO(
                        connectionString,
                        sp.GetRequiredService<PostgresWorldDAO>()
                    )
                )
                .AddScoped<PostgresPlayerLocationDAO>(sp =>
                    new PostgresPlayerLocationDAO(
                        connectionString,
                        sp.GetRequiredService<PostgresWorldDAO>(),
                        sp.GetRequiredService<PostgresMapDAO>(),
                        sp.GetRequiredService<PostgresPlayerDAO>()
                    )
                );
        }

        private static void RegisterHandlers(IServiceCollection services)
        {
            services
                .AddSingleton<SessionHandler>()
                .AddSingleton<ChatHandler>()
                .AddSingleton<AuthHandler>()
                .AddSingleton<MessageHandler>();
        }

        private static void RegisterManagers(IServiceCollection services)
        {
            services
                .AddSingleton<SessionManager>()
                .AddSingleton<ChatManager>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services
                .AddSingleton<PasswordService>()
                .AddSingleton<WorldService>()
                .AddSingleton<MapService>()
                .AddSingleton<PlayerService>()
                .AddSingleton<PlayerLocationService>()
                .AddSingleton<GlobalChatService>()
                .AddSingleton<ModerationService>()
                .AddSingleton<AccountService>()
                .AddSingleton<ResponseService>();

        }

        private static void RegisterModels(IServiceCollection services)
        {
            services
                .AddSingleton<PlayerModel>()
                .AddSingleton<PlayerLocation>()
                .AddSingleton<MapModel>()
                .AddSingleton<WorldModel>();
        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services
                .AddSingleton<WebSocketServerController>()
                .AddSingleton<AuthController>()
                .AddSingleton<AuthController>()
                .AddSingleton(sp =>
                    new Lazy<WebSocketServerController>(
                        () => sp.GetRequiredService<WebSocketServerController>()
                    )
                );
        }
    }
}

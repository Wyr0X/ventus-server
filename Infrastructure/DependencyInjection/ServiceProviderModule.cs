using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using VentusServer.Controllers;
using VentusServer.DataAccess.Postgres;
using VentusServer.Services;
using VentusServer.DataAccess;
using Game.Models;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Dapper;
using VentusServer.Controllers.Admin;

namespace VentusServer
{
    public static class ServiceProviderModule
    {
        public static ServiceProviderContainer Build(string firebaseCredentialsPath, string postgresConnectionString)
        {
            var services = new ServiceCollection();

            RegisterInfrastructure(services, firebaseCredentialsPath, postgresConnectionString);
            RegisterDAOs(services, postgresConnectionString);
            RegisterHandlers(services);
            RegisterManagers(services);
            RegisterServices(services);
            RegisterModels(services);
            RegisterControllers(services);
            var provider = services.BuildServiceProvider();

            return new ServiceProviderContainer(services, provider);
        }


        private static void RegisterInfrastructure(IServiceCollection services, string firebaseCredentialsPath, string postgresConnectionString)
        {
            services
                .AddSingleton<PostgresDbService>()
                .AddSingleton<FirebaseService>(sp => new FirebaseService(firebaseCredentialsPath))
                .AddSingleton<JwtService>()
                .AddSingleton<GameServer>()
                .AddSingleton<DatabaseInitializer>()
                .AddSingleton<ConcurrentDictionary<string, WebSocket>>()
                .AddSingleton<MessageSender>()
                .AddSingleton<RequirePermissionAttribute>()
                .AddSingleton(provider => new Lazy<MessageSender>(provider.GetRequiredService<MessageSender>))
                .AddSingleton<IDbConnectionFactory>(sp =>
                    new NpgsqlConnectionFactory(
                        postgresConnectionString
                    )
                );
        }

        private static void RegisterDAOs(IServiceCollection services, string connectionString)
        {

            // Ahora usamos la fábrica en vez del connectionString
            services
                .AddSingleton<IPlayerDAO>(sp =>
                    new DapperPlayerDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )

                .AddSingleton<IAccountDAO>(sp =>
                    new DapperAccountDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )

                .AddSingleton<IWorldDAO>(sp =>
                    new DapperWorldDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IMapDAO>(sp =>
                    new DapperMapDAO(sp.GetRequiredService<IDbConnectionFactory>(), sp.GetRequiredService<IWorldDAO>())
                )

                .AddSingleton<IPlayerLocationDAO>(sp =>
                    new DapperPlayerLocationDAO(
                        sp.GetRequiredService<IDbConnectionFactory>(),
                        sp.GetRequiredService<IPlayerDAO>(),
                        sp.GetRequiredService<IWorldDAO>(),
                        sp.GetRequiredService<IMapDAO>()
                    )
                )
                .AddSingleton<IRoleDAO>(sp =>
                    new DapperRoleDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IItemDAO>(sp =>
                    new DapperItemDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IPlayerStatsDAO>(sp =>
                    new DapperPlayerStatsDAO(sp.GetRequiredService<IDbConnectionFactory>())
                );
        }

        private static void RegisterHandlers(IServiceCollection services)
        {

        }

        private static void RegisterManagers(IServiceCollection services)
        {

        }

        private static void RegisterServices(IServiceCollection services)
        {
            services

                .AddSingleton<PasswordService>()
                .AddSingleton<WorldService>()
                .AddSingleton<MapService>()
                .AddSingleton<PlayerService>()
                .AddSingleton<PlayerLocationService>()
                .AddSingleton<AccountService>()
                .AddSingleton<ResponseService>()
                .AddSingleton<RoleService>()
                .AddSingleton<PermissionService>()
                .AddSingleton<ItemService>()
                .AddSingleton<PlayerStatsService>();

        }

        private static void RegisterModels(IServiceCollection services)
        {
            services
                .AddSingleton<PlayerModel>()
                .AddSingleton<PlayerLocationModel>()
                .AddSingleton<MapModel>()
                .AddSingleton<WorldModel>()
                .AddSingleton<RoleModel>();

        }

        private static void RegisterControllers(IServiceCollection services)
        {
            services
                .AddSingleton<WebSocketServerController>()
                .AddSingleton<AccountController>()
                .AddSingleton<AuthController>()
                .AddSingleton<AdminAccountController>()
                .AddSingleton<AdminRolesController>()
                .AddSingleton<AdminLogController>()
                .AddSingleton<AdminItemController>()

                .AddSingleton(sp =>
                    new Lazy<WebSocketServerController>(
                        () => sp.GetRequiredService<WebSocketServerController>()
                    )
                );
        }
    }
}

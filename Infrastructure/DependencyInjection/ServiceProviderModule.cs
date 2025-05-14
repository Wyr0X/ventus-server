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
using VentusServer.Domain.Models;
using VentusServer.DataAccess.DAO;
using Game.Server;

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
            RegisterControllers(services);
            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<SessionTasks>();
            provider.GetRequiredService<WorldTasks>();
            return new ServiceProviderContainer(services, provider);
        }


        private static void RegisterInfrastructure(IServiceCollection services, string firebaseCredentialsPath, string postgresConnectionString)
        {
            services
                .AddSingleton<PostgresDbService>()
                .AddSingleton<GameServer>()
                .AddSingleton<DatabaseInitializer>()
                .AddSingleton<TaskScheduler>()
                .AddSingleton<SessionTasks>()
                .AddSingleton<WorldTasks>()
                .AddSingleton<GameServiceMediator>()
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
                    new DapperMapDAO(sp.GetRequiredService<IDbConnectionFactory>())
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
                .AddSingleton<ISpellDAO>(sp =>
                    new DapperSpellDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IItemDAO>(sp =>
                    new DapperItemDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IPlayerInventoryDAO>(sp =>
                    new DapperPlayerInventoryDAO(sp.GetRequiredService<IDbConnectionFactory>())
                )
                .AddSingleton<IPlayerSpellsDAO>(sp =>
                    new DapperPlayerSpellsDAO(sp.GetRequiredService<IDbConnectionFactory>())
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
                .AddSingleton<IAccountService, AccountService>()
                .AddSingleton<ResponseService>()
                .AddSingleton<RoleService>()
                .AddSingleton<PermissionService>()
                .AddSingleton<ItemService>()
                .AddSingleton<PlayerInventoryService>()
                .AddSingleton<StoreService>()
                .AddSingleton<SpellService>()
                .AddSingleton<PlayerSpellsService>()
                .AddSingleton<PlayerStatsService>();

        }

        private static void RegisterControllers(IServiceCollection services)
        {
            try
            {
                services
                 .AddSingleton<WebSocketServerController>()
                 .AddSingleton<AccountController>()
                 .AddSingleton<AuthController>()
                 .AddSingleton<AdminAccountController>()
                 .AddSingleton<AdminRolesController>()
                 .AddSingleton<AdminLogController>()
                 .AddSingleton<AdminItemController>()
                 .AddSingleton<GameController>()
                 .AddSingleton<ItemController>()
                 .AddSingleton<StoreController>()
                 .AddSingleton<StoreController>()
                 .AddSingleton<SpellController>()
                 .AddSingleton(sp =>
                     new Lazy<WebSocketServerController>(
                         () => sp.GetRequiredService<WebSocketServerController>()
                     )
                 );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar los controladores.", ex.Message);
                throw;
            }
        }
    }
}

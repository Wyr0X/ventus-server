using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VentusServer.DataAccess.Postgres;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using VentusServer.Controllers;
using VentusServer.Services;
using VentusServer.Auth;

namespace VentusServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Obtener las credenciales de PostgreSQL desde las variables de entorno
            string host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
            string username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "password";
            string dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "ventus";
            Console.WriteLine("🚀 Iniciando configuración de servicios...");
            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(dbName))
            {
                Console.WriteLine("❌ No se encontraron las credenciales necesarias de PostgreSQL.");
                return;
            }
            string credentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH") ?? string.Empty;
            if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            {
                Console.WriteLine("❌ No se encontró el archivo de credenciales de Firebase.");
                return;
            }
            // Construir la cadena de conexión de PostgreSQL
            string postgresConnectionString = $"Host={host};Username={username};Password={password};Database={dbName}";
            services.AddSingleton<FirebaseService>(sp => new FirebaseService(credentialsPath));

            // Configuración CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Registrar servicios
            services.AddSingleton<JwtService>(); // Registrar JWTService
            services.AddSingleton<JwtAuthRequiredAttribute>(); // Registrar JWTService
            services.AddSingleton<PasswordService>();

            services.AddSingleton<PostgresAccountDAO>(sp => new PostgresAccountDAO(postgresConnectionString));
            services.AddSingleton<PostgresDbService>(sp => new PostgresDbService());
            services.AddSingleton<PostgresPlayerDAO>(sp => new PostgresPlayerDAO(postgresConnectionString));
            services.AddSingleton<PostgresWorldDAO>(sp => new PostgresWorldDAO(postgresConnectionString));
            services.AddSingleton<PostgresMapDAO>(sp =>
                  new PostgresMapDAO(postgresConnectionString, sp.GetRequiredService<PostgresWorldDAO>()));
            services.AddSingleton<PostgresPlayerLocationDAO>(sp => new PostgresPlayerLocationDAO(postgresConnectionString, sp.GetRequiredService<PostgresWorldDAO>()
            , sp.GetRequiredService<PostgresMapDAO>(), sp.GetRequiredService<PostgresPlayerDAO>()));



            services.AddSingleton<PlayerLocationService>();
            services.AddSingleton<MapService>();
            services.AddSingleton<WorldService>();
            services.AddSingleton<PlayerController>();

            services.AddSingleton<PlayerService>();

            services.AddSingleton<AccountController>();
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Middleware de CORS
            app.UseCors("AllowAll");

            // Middleware de Autenticación de Firebase (aplicado a todas las rutas excepto las públicas)
       //     app.UseMiddleware<FirebaseAuthMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                Console.WriteLine("Endpoints configurados correctamente");
            });
        }
    }
}

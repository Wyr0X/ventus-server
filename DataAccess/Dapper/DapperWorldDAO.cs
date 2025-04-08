using Dapper;
using Game.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.DataAccess.Queries;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperWorldDAO(IDbConnectionFactory connectionFactory) : BaseDAO(connectionFactory), IWorldDAO
    {
        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int maxPlayers, int levelRequirements)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, "Iniciando la creación del mundo...");

            using var connection = _connectionFactory.CreateConnection();

            try
            {
                // Log de los parámetros antes de la consulta
                LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, $"Ejecutando consulta con parámetros: Name = {name}, Description = {description}, MaxMaps = {maxMaps}, MaxPlayers = {maxPlayers}, LevelRequirements = {levelRequirements}");

                var row = await connection.QuerySingleOrDefaultAsync(WorldQueries.Insert, new
                {
                    Name = name,
                    Description = description,
                    MaxMaps = maxMaps,
                    MaxPlayers = maxPlayers,
                    LevelRequirements = levelRequirements
                });

                if (row == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, "No se encontró el resultado al ejecutar la consulta.");
                    return null;
                }

                // Log después de la consulta
                LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, "Consulta ejecutada con éxito, mapeando resultado...");
                WorldModel worldModel = WorldMapper.Map(row);

                if (worldModel == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, "El mapeo de la fila ha fallado, el modelo resultante es null.");
                    return null;
                }

                // Log final

                worldModel.Id = row.id;
                LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, $"Mundo creado exitosamente. ${worldModel.ToString()}");
                worldModel.PrintInfo();
                return worldModel;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperWorldDAO, $"Error al crear el mundo: {ex.Message}");
                throw; // Re-lanzar la excepción para manejarla más arriba
            }
        }

        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            using var connection = _connectionFactory.CreateConnection();
            var row = await connection.QuerySingleOrDefaultAsync(WorldQueries.SelectById, new { WorldId = worldId });

            return row == null ? null : WorldMapper.Map(row);
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.QueryAsync(WorldQueries.SelectAll);

            return WorldMapper.MapRowsToWorlds(rows);
        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            using var connection = _connectionFactory.CreateConnection();
            var entity = WorldMapper.ToEntity(world);
            await connection.ExecuteAsync(WorldQueries.Upsert, entity);
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(WorldQueries.Delete, new { WorldId = worldId });
        }

        public class WorldInitializer(IDbConnectionFactory connectionFactory)
        {
            private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

            public async Task InitializeWorldTableAsync()
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(WorldQueries.CreateTableQuery);
            }
        }
    }
}

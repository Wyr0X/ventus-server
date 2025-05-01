using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Models;
using VentusServer.DataAccess.Mappers;
using static LoggerUtil;
using VentusServer.DataAccess.Queries;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperPlayerStatsDAO : BaseDAO, IPlayerStatsDAO
    {
        public DapperPlayerStatsDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        // Obtener las estadísticas de un jugador por su ID
        public async Task<PlayerStatsModel?> GetPlayerStatsByIdAsync(int playerId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Buscando estadísticas de jugador por ID:  {playerId}");

            if (playerId <= 0) return null;

            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Buscando estadísticas de jugador por ID:  {playerId}");
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(PlayerStatsQueries.SelectByPlayerId, new { PlayerId = playerId });
                PlayerStatsMapper.PrintRow(row);
                return row == null ? null : PlayerStatsMapper.FromRow(row);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Error en GetPlayerStatsByIdAsync: {ex.Message}");
                return null;
            }
        }


        // Crear estadísticas para un nuevo jugador
        public async Task<PlayerStatsModel> CreatePlayerStatsAsync(int playerId)
        {
            var newPlayerStats = new PlayerStatsModel
            {
                PlayerId = playerId,
                Level = 1,
                Xp = 0,
                Gold = 0,
                BankGold = 0,
                FreeSkillPoints = 0,
                Hp = 100,
                Mp = 50,
                Sp = 10,
                MaxHp = 100,
                MaxMp = 50,
                MaxSp = 10,
                Hunger = 100,
                Thirst = 100,
                KilledNpcs = 0,
                KilledUsers = 0,
                Deaths = 0,
                LastUpdated = DateTime.UtcNow
            };

            try
            {
                var entity = PlayerStatsMapper.ToEntity(newPlayerStats);

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(PlayerStatsQueries.Insert, entity);
                return newPlayerStats;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Error creando estadísticas de jugador: {ex.Message}");
                throw;
            }
        }

        // Actualizar las estadísticas de un jugador
        public async Task SavePlayerStatsAsync(PlayerStatsModel playerStats)
        {
            try
            {
                var entity = PlayerStatsMapper.ToEntity(playerStats);

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(PlayerStatsQueries.Upsert, entity);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Error guardando estadísticas de jugador: {ex.Message}");
                throw;
            }
        }

        // Eliminar las estadísticas de un jugador
        public async Task<bool> DeletePlayerStatsAsync(int playerId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var affected = await connection.ExecuteAsync(PlayerStatsQueries.DeleteByPlayerId, new { PlayerId = playerId });
                return affected > 0;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Error eliminando estadísticas de jugador: {ex.Message}");
                return false;
            }
        }

        // Verificar si las estadísticas de un jugador existen
        public async Task<bool> PlayerStatsExistsAsync(int playerId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<bool>(PlayerStatsQueries.ExistsByPlayerId, new { PlayerId = playerId });
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerStatsDAO, $"Error en PlayerStatsExistsAsync: {ex.Message}");
                return false;
            }
        }

        public Task<PlayerStatsModel?> GetPlayerStatsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerStatsModel>> GetAllPlayerStatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerStatsModel>> GetPlayerStatsByAccountIdAsync(Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}

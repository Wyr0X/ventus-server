using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Mappers;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.DAO
{


    public class DapperPlayerSpellsDAO : IPlayerSpellsDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DapperPlayerSpellsDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PlayerSpellsModel?> GetByIdAsync(int id)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.SelectById}, with id: {id}");

            var row = await conn.QueryFirstOrDefaultAsync(
                PlayerSpellsQueries.SelectById,
                new { Id = id }
            );

            if (row != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Query successful, result: {JsonSerializer.Serialize(row)}");
                return PlayerSpellsMapper.MapFromFrow(row);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, "Query returned no result.");
                return null;
            }
        }
        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<PlayerSpellsModel?> GetByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.SelectByPlayerId}, with playerId: {playerId}");

            var row = await conn.QueryFirstOrDefaultAsync(
                PlayerSpellsQueries.SelectByPlayerId,
                new { PlayerId = playerId }
            );

            if (row != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Query successful, result: {JsonSerializer.Serialize(row)}");
                return PlayerSpellsMapper.MapFromFrow(row);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, "Query returned no result.");
                return null;
            }
        }

        public async Task CreateAsync(PlayerSpellsModel model)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.Insert}, with model: {JsonSerializer.Serialize(model)}");

            var id = await conn.ExecuteScalarAsync<int>(PlayerSpellsQueries.Insert, new
            {
                model.PlayerId,
                Spells = JsonSerializer.Serialize(model.Spells),
                model.MaxSlots,
                model.CreatedAt,
                model.UpdatedAt
            });

            model.Id = id;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Inserted new spell inventory with ID: {id}");
        }

        public async Task UpdateSpells(int playerId, List<PlayerSpellModel> spells)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.UpdateSpells}, with playerId: {playerId}, spells count: {spells.Count}");

            var serialized = JsonSerializer.Serialize(spells);
            await conn.ExecuteAsync(
                PlayerSpellsQueries.UpdateSpells,
                new
                {
                    PlayerId = playerId,
                    Spells = serialized,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Updated spells for playerId: {playerId}");
        }

        public async Task UpdateMaxSlots(int playerId, int maxSlots)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.UpdateMaxSlots}, with playerId: {playerId}, maxSlots: {maxSlots}");

            await conn.ExecuteAsync(
                PlayerSpellsQueries.UpdateMaxSlots,
                new
                {
                    PlayerId = playerId,
                    MaxSlots = maxSlots,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Updated max slots for playerId: {playerId}, new maxSlots: {maxSlots}");
        }

        public async Task UpsertAsync(PlayerSpellsModel model)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Upserting spell inventory for playerId: {model.PlayerId}");

            if (!await ExistsByPlayerId(model.PlayerId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, "Spell inventory does not exist, creating new spell inventory.");
                await CreateAsync(model);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, "Spell inventory exists, updating spells and max slots.");
                await UpdateSpells(model.PlayerId, model.Spells);
                await UpdateMaxSlots(model.PlayerId, model.MaxSlots);
            }
        }

        public async Task DeleteByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.DeleteByPlayerId}, with playerId: {playerId}");

            await conn.ExecuteAsync(
                PlayerSpellsQueries.DeleteByPlayerId,
                new { PlayerId = playerId }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Deleted spell inventory for playerId: {playerId}");
        }

        public async Task<bool> ExistsByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Executing query: {PlayerSpellsQueries.ExistsByPlayerId}, with playerId: {playerId}");

            var exists = await conn.QueryFirstAsync<bool>(
                PlayerSpellsQueries.ExistsByPlayerId,
                new { PlayerId = playerId }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerSpellsDAO, $"Spell inventory exists for playerId: {playerId}: {exists}");
            return exists;
        }
    }
}


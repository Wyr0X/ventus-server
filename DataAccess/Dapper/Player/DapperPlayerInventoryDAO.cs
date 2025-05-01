// VentusServer.DataAccess.DAO/DapperPlayerInventoryDAO.cs

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
    public class DapperPlayerInventoryDAO : IPlayerInventoryDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DapperPlayerInventoryDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<PlayerInventoryModel?> GetByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.SelectByPlayerId}, with playerId: {playerId}");

            var row = await conn.QueryFirstOrDefaultAsync(
                PlayerInventoryQueries.SelectByPlayerId,
                new { PlayerId = playerId }
            );

            if (row != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Query successful, result: {JsonSerializer.Serialize(row)}");
                return PlayerInventoryMapper.MapFromFrow(row);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, "Query returned no result.");
                return null;
            }
        }

        public async Task CreateAsync(PlayerInventoryModel model)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.Insert}, with model: {JsonSerializer.Serialize(model)}");

            var id = await conn.ExecuteScalarAsync<int>(PlayerInventoryQueries.Insert, new
            {
                model.PlayerId,
                model.Gold,
                Items = JsonSerializer.Serialize(model.Items),
                model.MaxSlots,

                model.CreatedAt,
                model.UpdatedAt
            });

            model.Id = id;
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Inserted new inventory with ID: {id}");
        }

        public async Task UpdateGold(int playerId, int gold)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.UpdateGold}, with playerId: {playerId}, gold: {gold}");

            await conn.ExecuteAsync(
                PlayerInventoryQueries.UpdateGold,
                new
                {
                    PlayerId = playerId,
                    Gold = gold,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Updated gold for playerId: {playerId}, new gold: {gold}");
        }

        public async Task UpdateItems(int playerId, List<PlayerInventoryItemModel> items)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.UpdateItems}, with playerId: {playerId}, items count: {items.Count}");

            var serialized = JsonSerializer.Serialize(items);
            await conn.ExecuteAsync(
                PlayerInventoryQueries.UpdateItems,
                new
                {
                    PlayerId = playerId,
                    Items = serialized,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Updated items for playerId: {playerId}");
        }

        public async Task UpsertAsync(PlayerInventoryModel model)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Upserting inventory for playerId: {model.PlayerId}");

            if (!await ExistsByPlayerId(model.PlayerId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, "Inventory does not exist, creating new inventory.");
                await CreateAsync(model);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, "Inventory exists, updating gold and items.");
                await UpdateGold(model.PlayerId, model.Gold);
                await UpdateItems(model.PlayerId, model.Items);
            }
        }

        public async Task DeleteByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.DeleteByPlayerId}, with playerId: {playerId}");

            await conn.ExecuteAsync(
                PlayerInventoryQueries.DeleteByPlayerId,
                new { PlayerId = playerId }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Deleted inventory for playerId: {playerId}");
        }

        public async Task<bool> ExistsByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Executing query: {PlayerInventoryQueries.ExistsByPlayerId}, with playerId: {playerId}");

            var exists = await conn.QueryFirstAsync<bool>(
                PlayerInventoryQueries.ExistsByPlayerId,
                new { PlayerId = playerId }
            );

            LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerInventoryDAO, $"Inventory exists for playerId: {playerId}: {exists}");
            return exists;
        }
    }
}

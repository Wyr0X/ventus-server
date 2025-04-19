using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using VentusServer.DataAccess.Mappers;
using VentusServer.DataAccess.Queries;
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
            var row = await conn.QueryFirstOrDefaultAsync(
                PlayerInventoryQueries.SelectByPlayerId,
                new { PlayerId = playerId }
            );
            return row != null ? PlayerInventoryMapper.Map(row) : null;
        }

        public async Task Insert(PlayerInventoryModel model)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                PlayerInventoryQueries.Insert,
                new
                {
                    model.Id,
                    model.PlayerId,
                    model.Gold,
                    model.CreatedAt,
                    model.UpdatedAt
                }
            );
        }

        public async Task UpdateGold(int playerId, int gold, DateTime updatedAt)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                PlayerInventoryQueries.UpdateGold,
                new { PlayerId = playerId, Gold = gold, UpdatedAt = updatedAt }
            );
        }

        public async Task DeleteByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            await conn.ExecuteAsync(
                PlayerInventoryQueries.DeleteByPlayerId,
                new { PlayerId = playerId }
            );
        }

        public async Task<bool> ExistsByPlayerId(int playerId)
        {
            using var conn = GetConnection();
            var exists = await conn.QueryFirstOrDefaultAsync<bool>(
                PlayerInventoryQueries.ExistsByPlayerId,
                new { PlayerId = playerId }
            );
            return exists;
        }
    }
}

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using VentusServer.DataAccess.Mappers;
using VentusServer.Domain.Models;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Interfaces;

namespace VentusServer.DataAccess.DAO
{
    public class DapperPlayerInventoryItemDAO : IPlayerInventoryItemDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        // Inyección de dependencias para obtener la conexión
        public DapperPlayerInventoryItemDAO(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private IDbConnection GetConnection() => _connectionFactory.CreateConnection();

        public async Task<List<PlayerInventoryItemModel>> GetAllByInventoryId(Guid inventoryId)
        {
            using var connection = GetConnection();

            var result = await connection.QueryAsync(
                PlayerItemQueries.SelectByInventoryId,
                new { InventoryId = inventoryId }
            );

            return PlayerInventoryItemMapper.MapRowsToModels(result);
        }

        public async Task<PlayerInventoryItemModel?> GetById(Guid id)
        {
            using var connection = GetConnection();

            var row = await connection.QueryFirstOrDefaultAsync(
                PlayerItemQueries.SelectById,
                new { Id = id }
            );

            return row != null ? PlayerInventoryItemMapper.Map(row) : null;
        }

        public async Task Insert(PlayerInventoryItemModel model)
        {
            using var connection = GetConnection();

            await connection.ExecuteAsync(
                PlayerItemQueries.Insert,
                new
                {
                    model.Id,
                    model.InventoryId,
                    model.ItemId,
                    model.Quantity,
                    model.Slot,
                    CustomData = model.CustomData?.RootElement.GetRawText(),
                    model.CreatedAt,
                    model.UpdatedAt
                }
            );
        }

        public async Task Update(PlayerInventoryItemModel model)
        {
            using var connection = GetConnection();

            await connection.ExecuteAsync(
                PlayerItemQueries.Update,
                new
                {
                    model.Id,
                    model.Quantity,
                    model.Slot,
                    CustomData = model.CustomData?.RootElement.GetRawText(),
                    model.UpdatedAt
                }
            );
        }

        public async Task Delete(Guid id)
        {
            using var connection = GetConnection();

            await connection.ExecuteAsync(
                PlayerItemQueries.Delete,
                new { Id = id }
            );
        }
    }
}

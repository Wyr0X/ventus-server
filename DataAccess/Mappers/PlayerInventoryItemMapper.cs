using System.Collections.Generic;
using System.Linq;
using Game.Models;
using VentusServer.DataAccess.Entities;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerInventoryItemMapper : BaseMapper
    {
        public static PlayerInventoryItemModel Map(dynamic row)
        {
            return new PlayerInventoryItemModel
            {
                Id = row.id,
                InventoryId = row.inventory_id,
                ItemId = row.item_id,
                Quantity = row.quantity,
                Slot = row.slot,
                CustomData = row.custom_data,
                CreatedAt = row.created_at,
                UpdatedAt = row.updated_at,

                // Datos opcionales que pueden venir de un JOIN con items
                Name = row.name is not null ? row.name : null,
                Icon = row.icon is not null ? row.icon : null,
                ItemType = row.item_type is not null ? row.item_type : null
            };
        }

        public static List<PlayerInventoryItemModel> MapRowsToModels(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        public static DbPlayerInventoryItemEntity ToEntity(PlayerInventoryItemModel model)
        {
            return new DbPlayerInventoryItemEntity
            {
                Id = model.Id,
                InventoryId = model.InventoryId,
                ItemId = model.ItemId,
                Quantity = model.Quantity,
                Slot = model.Slot,
                CustomData = model.CustomData,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }

        public static PlayerInventoryItemModel ToModel(DbPlayerInventoryItemEntity entity)
        {
            return new PlayerInventoryItemModel
            {
                Id = entity.Id,
                InventoryId = entity.InventoryId,
                ItemId = entity.ItemId,
                Quantity = entity.Quantity,
                Slot = entity.Slot,
                CustomData = entity.CustomData,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

                Name = null,
                Icon = null,
                ItemType = null
            };
        }
    }
}

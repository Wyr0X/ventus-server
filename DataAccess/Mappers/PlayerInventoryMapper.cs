using System.Collections.Generic;
using System.Linq;
using Game.Models;
using VentusServer.DataAccess.Entities;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerInventoryMapper : BaseMapper
    {
        public static PlayerInventoryModel Map(dynamic row)
        {
            return new PlayerInventoryModel
            {
                Id = row.id,
                PlayerId = row.player_id,
                Gold = row.gold,
                CreatedAt = row.created_at,
                UpdatedAt = row.updated_at,
                Items = new List<PlayerInventoryItemModel>() // Se asignan luego desde el DAO
            };
        }

        public static List<PlayerInventoryModel> MapRowsToModels(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        public static DbPlayerInventoryEntity ToEntity(PlayerInventoryModel model)
        {
            return new DbPlayerInventoryEntity
            {
                Id = model.Id,
                PlayerId = model.PlayerId,
                Gold = model.Gold,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }

        public static PlayerInventoryModel ToModel(DbPlayerInventoryEntity entity)
        {
            return new PlayerInventoryModel
            {
                Id = entity.Id,
                PlayerId = entity.PlayerId,
                Gold = entity.Gold,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Items = new List<PlayerInventoryItemModel>() // Se asignan luego desde el DAO
            };
        }
    }
}

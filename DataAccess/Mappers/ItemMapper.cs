using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Mappers
{
    public class ItemMapper : BaseMapper
    {
        public static ItemModel Map(dynamic row)
        {
            return new ItemModel
            {
                Id = row.id,
                Key = row.key,
                Name = row.name,
                Description = row.description,
                HpMin = row.hp_min,
                HpMax = row.hp_max,
                MP = row.MP,
                Sound = row.sound,
                CreatedAt = row.created_at
            };
        }

        public static List<ItemModel> MapRowsToItems(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        public static DbItemEntity ToEntity(ItemModel model)
        {
            return new DbItemEntity
            {
                Id = model.Id,
                Key = model.Key,
                Name = model.Name,
                Description = model.Description,
                HpMin = model.HpMin,
                HpMax = model.HpMax,
                MP = model.MP,
                Sound = model.Sound,
                CreatedAt = model.CreatedAt
            };
        }

        public static ItemModel ToModel(DbItemEntity entity)
        {
            return new ItemModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = entity.Name,
                Description = entity.Description,
                HpMin = entity.HpMin,
                HpMax = entity.HpMax,
                MP = entity.MP,
                Sound = entity.Sound,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}

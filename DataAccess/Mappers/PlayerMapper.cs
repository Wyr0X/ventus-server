using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerMapper : BaseMapper
    {
        // 🔄 De fila dinámica a modelo (int → enum)
        public static PlayerModel FromRow(dynamic row)
        {
            return new PlayerModel(
                id: row.id,
                accountId: row.account_id,
                name: row.name,
                gender: (Gender)row.gender,
                race: (Race)row.race,
                level: row.level,
                playerClass: row.@class
            )
            {
                CreatedAt = row.created_at,
                LastLogin = row.last_login,
                Status = row.status
            };
        }

        public static List<PlayerModel> FromRows(IEnumerable<dynamic> rows) =>
            rows.Select(FromRow).ToList();

        // 🔄 De modelo a entidad (enum → int)
        public static DbPlayerEntity ToEntity(PlayerModel model)
        {
            return new DbPlayerEntity
            {
                Id = model.Id,
                AccountId = model.AccountId,
                Name = model.Name,
                Gender = (int)model.Gender,
                Race = (int)model.Race,
                Level = model.Level,
                Class = model.Class,
                CreatedAt = model.CreatedAt,
                LastLogin = model.LastLogin,
                Status = model.Status
            };
        }

        // 🔄 De entidad a modelo (int → enum)
        public static PlayerModel ToModel(DbPlayerEntity entity)
        {
            return new PlayerModel(
                id: entity.Id,
                accountId: entity.AccountId,
                name: entity.Name,
                gender: (Gender)entity.Gender,
                race: (Race)entity.Race,
                level: entity.Level,
                playerClass: entity.Class
            )
            {
                CreatedAt = entity.CreatedAt,
                LastLogin = entity.LastLogin,
                Status = entity.Status,
                isSpawned = false
            };
        }
    }
}

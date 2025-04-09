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

            return new PlayerModel
            {
                Id = row.id,
                AccountId = row.account_id,
                Name = row.name,
                Gender = (Gender)row.gender,
                Race = (Race)row.race,
                Level = row.level,
                Class = (CharacterClass)row.@class,

                CreatedAt = row.created_at,
                LastLogin = row.last_login,
                Status = row.status                 // Asignación adicional
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
                Class = (int)model.Class,
                CreatedAt = model.CreatedAt,
                LastLogin = model.LastLogin,
                Status = model.Status
            };
        }

        // 🔄 De entidad a modelo (int → enum)
        public static PlayerModel ToModel(DbPlayerEntity entity)
        {

            return new PlayerModel
            {
                Id = entity.Id,
                AccountId = entity.AccountId,
                Name = entity.Name,
                Gender = (Gender)entity.Gender,
                Race = (Race)entity.Race,
                Level = entity.Level,
                Class = (CharacterClass)entity.Class,
                CreatedAt = entity.CreatedAt,
                LastLogin = entity.LastLogin,
                Status = entity.Status,
                isSpawned = false

            };
        }
    }
}

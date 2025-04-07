using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerMapper  : BaseMapper
    {
        // 🧭 Mapeo desde un resultado dinámico de Dapper a un modelo de jugador
        public static PlayerModel FromRow(dynamic row)
        {
            return new PlayerModel(
                id: row.id,
                accountId: row.account_id,
                name: row.name,
                gender: row.gender,
                race: row.race,
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

        // 🧭 De modelo a entidad de base de datos
        public static DbPlayerEntity ToEntity(PlayerModel model)
        {
            return new DbPlayerEntity
            {
                Id = model.Id,
                AccountId = model.AccountId,
                Name = model.Name,
                Gender = model.Gender,
                Race = model.Race,
                Level = model.Level,
                Class = model.Class,
                CreatedAt = model.CreatedAt,
                LastLogin = model.LastLogin,
                Status = model.Status
            };
        }

        // 🧭 De entidad de base de datos a modelo
        public static PlayerModel ToModel(DbPlayerEntity entity)
        {
            return new PlayerModel(
                id: entity.Id,
                accountId: entity.AccountId,
                name: entity.Name,
                gender: entity.Gender,
                race: entity.Race,
                level: entity.Level,
                playerClass: entity.Class
            )
            {
                CreatedAt = entity.CreatedAt,
                LastLogin = entity.LastLogin,
                Status = entity.Status,

                // ⚠️ Vive solo en memoria (no se guarda en DB)
                isSpawned = false
            };
        }
    }
}

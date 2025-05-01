using System;
using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerStatsMapper : BaseMapper
    {
        // 🧭 Mapeo desde un resultado dinámico de Dapper a un modelo de estadísticas de jugador
        public static PlayerStatsModel FromRow(dynamic row)
        {
            return new PlayerStatsModel
            {
                PlayerId = row.player_id,
                Level = row.level,
                Xp = row.xp,
                Gold = row.gold,
                BankGold = row.bank_gold,
                FreeSkillPoints = row.free_skill_points,
                Hp = row.hp,
                Mp = row.mp,
                Sp = row.sp,
                MaxHp = row.max_hp,
                MaxMp = row.max_mp,
                MaxSp = row.max_sp,
                Hunger = row.hunger,
                Thirst = row.thirst,
                KilledNpcs = row.killed_npcs,
                KilledUsers = row.killed_users,
                Deaths = row.deaths,
                LastUpdated = row.last_updated // Este campo vive solo en memoria
            };
        }

        public static List<PlayerStatsModel> FromRows(IEnumerable<dynamic> rows) =>
            rows.Select(FromRow).ToList();

        // 🧭 De modelo a entidad de base de datos
        public static DBStatsEntity ToEntity(PlayerStatsModel model)
        {
            return new DBStatsEntity
            {
                PlayerId = model.PlayerId,
                Level = model.Level,
                Xp = model.Xp,
                Gold = model.Gold,
                BankGold = model.BankGold,
                FreeSkillPoints = model.FreeSkillPoints,
                Hp = model.Hp,
                Mp = model.Mp,
                Sp = model.Sp,
                MaxHp = model.MaxHp,
                MaxMp = model.MaxMp,
                MaxSp = model.MaxSp,
                Hunger = model.Hunger,
                Thirst = model.Thirst,
                KilledNpcs = model.KilledNpcs,
                KilledUsers = model.KilledUsers,
                Deaths = model.Deaths
            };
        }

        // 🧭 De entidad de base de datos a modelo
        public static PlayerStatsModel ToModel(DBStatsEntity entity)
        {
            return new PlayerStatsModel
            {
                PlayerId = entity.PlayerId,
                Level = entity.Level,
                Xp = entity.Xp,
                Gold = entity.Gold,
                BankGold = entity.BankGold,
                FreeSkillPoints = entity.FreeSkillPoints,
                Hp = entity.Hp,
                Mp = entity.Mp,
                Sp = entity.Sp,
                MaxHp = entity.MaxHp,
                MaxMp = entity.MaxMp,
                MaxSp = entity.MaxSp,
                Hunger = entity.Hunger,
                Thirst = entity.Thirst,
                KilledNpcs = entity.KilledNpcs,
                KilledUsers = entity.KilledUsers,
                Deaths = entity.Deaths,

                // ⚠️ Vive solo en memoria (no se guarda en DB)
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}

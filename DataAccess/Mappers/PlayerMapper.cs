using System.Collections.Generic;
using System.Linq;
using VentusServer.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class PlayerMapper
    {
        public static PlayerModel Map(dynamic row)
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

        public static PlayerModel MapRowToPlayer(dynamic row)
        {
            return Map(row);
        }

        public static List<PlayerModel> MapRowsToPlayers(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }
    }
}

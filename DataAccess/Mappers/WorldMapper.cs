using System.Collections.Generic;
using System.Linq;
using Game.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class WorldMapper
    {
        public static WorldModel Map(dynamic row)
        {
            return new WorldModel
            {
                Id = row.id,
                Name = row.name,
                Description = row.description,
                MaxMaps = row.max_maps,
                MaxPlayers = row.max_players,
                LevelRequirements = row.level_requirements
            };
        }

        public static WorldModel MapRowToWorld(dynamic row)
        {
            return Map(row);
        }

        public static List<WorldModel> MapRowsToWorlds(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }
    }
}

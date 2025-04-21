using System.Collections.Generic;
using System.Linq;
using Game.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class MapMapper : BaseMapper
    {
        public static MapModel Map(dynamic row)
        {
            return new MapModel
            {
                Id = row.id,
                Name = row.name,
                MinLevel = row.min_level,
                MaxPlayers = row.max_players,
                WorldId = row.world_id,
                // El WorldModel será asignado aparte en el DAO
                WorldModel = null,
                // Estas listas son lógicas y se mantienen en memoria
                PlayersLocation = new List<PlayerLocationModel>(),
                spawnedPlayers = new List<PlayerLocationModel>()
            };
        }

        public static List<MapModel> MapRowsToMaps(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        // Convierte un modelo en una entidad de base de datos (para insertar o actualizar)
        public static DbMapEntity ToEntity(MapModel model)
        {
            return new DbMapEntity
            {
                Id = model.Id,
                Name = model.Name,
                MinLevel = model.MinLevel,
                MaxPlayers = model.MaxPlayers,
                WorldId = model.WorldId
            };
        }

        // Convierte una entidad de base de datos en modelo (por si accedes directo a una entidad)
        public static MapModel ToModel(DbMapEntity entity)
        {
            return new MapModel
            {
                Id = entity.Id,
                Name = entity.Name,
                MinLevel = entity.MinLevel,
                MaxPlayers = entity.MaxPlayers,
                WorldId = entity.WorldId,
                WorldModel = null,
                PlayersLocation = new List<PlayerLocationModel>(),
                spawnedPlayers = new List<PlayerLocationModel>()
            };
        }
    }
}

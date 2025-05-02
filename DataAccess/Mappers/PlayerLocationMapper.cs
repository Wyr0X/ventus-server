using Game.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerLocationMapper : BaseMapper
    {
        // Desde fila dinámica (por ejemplo, de Dapper) a modelo completo
        public static PlayerLocationModel Map(dynamic dbRow, PlayerModel player, WorldModel world, MapModel map)
        {
            return new PlayerLocationModel
            {
                PosX = dbRow.pos_x,
                PosY = dbRow.pos_y,
                PlayerId = player.Id,
                WorldId = world.Id,
                MapId = map.Id
            };
        }

        // Desde entidad a modelo (por si se usa un ORM o Dapper sin dynamic)
        public static PlayerLocationModel ToModel(DbLocationEntity entity, PlayerModel player, WorldModel world, MapModel map)
        {
            return new PlayerLocationModel
            {
                PosX = entity.PosX,
                PosY = entity.PosY,
                PlayerId = player.Id,
                WorldId = world.Id,
                MapId = map.Id
            };
        }

        // De modelo de juego a entidad de base de datos
        public static DbLocationEntity ToEntity(PlayerLocationModel model)
        {
            return new DbLocationEntity
            {
                PlayerId = model.PlayerId,
                WorldId = model.WorldId,
                MapId = model.MapId,
                PosX = model.PosX,
                PosY = model.PosY,
            };
        }

        // Si seguís usando parámetros anónimos para Dapper
        public static object ToDbParameters(PlayerLocationModel location)
        {
            return new
            {
                PlayerId = location.PlayerId,
                WorldId = location.WorldId,
                MapId = location.MapId,
                PosX = location.PosX,
                PosY = location.PosY,
            };
        }
    }
}

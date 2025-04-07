using Game.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class PlayerLocationMapper  : BaseMapper
    {
        // Desde fila dinámica (por ejemplo, de Dapper) a modelo completo
        public static PlayerLocation Map(dynamic dbRow, PlayerModel player, WorldModel world, MapModel map)
        {
            return new PlayerLocation
            {
                PosX = dbRow.pos_x,
                PosY = dbRow.pos_y,
                Player = player,
                World = world,
                Map = map
            };
        }

        // Desde entidad a modelo (por si se usa un ORM o Dapper sin dynamic)
        public static PlayerLocation ToModel(DbLocationEntity entity, PlayerModel player, WorldModel world, MapModel map)
        {
            return new PlayerLocation
            {
                PosX = entity.PosX,
                PosY = entity.PosY,
                Player = player,
                World = world,
                Map = map
            };
        }

        // De modelo de juego a entidad de base de datos
        public static DbLocationEntity ToEntity(PlayerLocation model)
        {
            return new DbLocationEntity
            {
                PlayerId = model.Player.Id,
                WorldId = model.World.Id,
                MapId = model.Map.Id,
                PosX = model.PosX,
                PosY = model.PosY,
            };
        }

        // Si seguís usando parámetros anónimos para Dapper
        public static object ToDbParameters(PlayerLocation location)
        {
            return new
            {
                PlayerId = location.Player.Id,
                WorldId = location.World.Id,
                MapId = location.Map.Id,
                PosX = location.PosX,
                PosY = location.PosY,
            };
        }
    }
}

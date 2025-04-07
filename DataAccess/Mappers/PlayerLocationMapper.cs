using Game.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class PlayerLocationMapper
    {
        public static PlayerLocation ToModel(dynamic dbRow, PlayerModel player, WorldModel world, MapModel map)
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

        public static object ToDbParameters(PlayerLocation location)
        {
            return new
            {
                PlayerId = location.Player.Id,
                WorldId = location.World.Id,
                MapId = location.Map.Id,
                PosX = location.PosX,
                PosY = location.PosY
            };
        }
    }
}

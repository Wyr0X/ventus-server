using VentusServer.Domain.Objects;

public static class EnvironmentHelper
{
    // Verifica si el jugador está en un terreno que afecta la acción
    public static bool IsInRestrictedTerrain(PlayerObject player)
    {
        return player.CurrentMap.IsWater(player.Position)
        || player.CurrentMap.IsLava(player.Position) || player.CurrentMap.IsImpassable(player.Position);
    }

    // // Verifica si hay línea de visión entre el jugador y el target
    // public static bool HasLineOfSight(PlayerObject player, Vec2 targetPosition)
    // {
    //     Vec2 start = player.Position;
    //     int x0 = (int)Math.Floor(start.X);
    //     int y0 = (int)Math.Floor(start.Y);
    //     int x1 = (int)Math.Floor(targetPosition.X);
    //     int y1 = (int)Math.Floor(targetPosition.Y);

    //     int dx = Math.Abs(x1 - x0);
    //     int dy = Math.Abs(y1 - y0);
    //     int sx = x0 < x1 ? 1 : -1;
    //     int sy = y0 < y1 ? 1 : -1;
    //     int err = dx - dy;

    //     while (true)
    //     {


    //         if (x0 == x1 && y0 == y1)
    //             break;

    //         int e2 = 2 * err;
    //         if (e2 > -dy)
    //         {
    //             err -= dy;
    //             x0 += sx;
    //         }
    //         if (e2 < dx)
    //         {
    //             err += dx;
    //             y0 += sy;
    //         }
    //     }

    //     return true;
    // }
}
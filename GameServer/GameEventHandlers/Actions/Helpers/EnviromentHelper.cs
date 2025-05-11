using VentusServer.Domain.Objects;

public static class EnvironmentHelper
{
    // Verifica si el jugador está en un terreno que afecta la acción
    public static bool IsInRestrictedTerrain(PlayerObject player)
    {
        return player.CurrentZoneContext.IsWater(player.Position.X, player.Position.Y)
        || player.CurrentZoneContext.IsLava(player.Position.X, player.Position.Y) || player.CurrentZoneContext.IsImpassable(player.Position.X, player.Position.Y);
    }

    // Verifica si hay línea de visión entre el jugador y el target
    public static bool HasLineOfSight(PlayerObject player, object target)
    {
        // Implementar lógica de raycasting o grid-based LOS
        return true; // Placeholder
    }
}
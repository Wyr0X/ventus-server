using VentusServer.Domain.Objects;

public static class GlobalStateHelper
{
    // Verifica si el jugador está en un estado que bloquea acciones
    public static bool IsPlayerInRestrictedState(PlayerObject player)
    {
        return player.Stats.IsDead
            || player.Stats.IsStunned
            || player.Stats.IsRooted
            || player.Stats.IsSilenced
            || player.Stats.IsChanneling;
    }

    // Verifica si el target tiene inmunidades o resistencias
    public static bool IsTargetImmune(object target, EffectType effect)
    {
        // if (target is PlayerObject targetPlayer)
        // {
        //     return targetPlayer.Stats.Immunities.Contains(effect); //Verlo con Ale
        // }
        // return false;
        return false;
    }

    // Verifica si el jugador está en una zona especial (safe zone, PvP restricted, etc.)
    public static bool IsInSpecialZone(PlayerObject player)
    {
        return player.CurrentZoneContext.IsSafeZone || !player.CurrentZoneContext.AllowsCombat;
    }
}
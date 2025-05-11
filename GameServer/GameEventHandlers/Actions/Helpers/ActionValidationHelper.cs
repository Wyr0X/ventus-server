using Server.Logic.Validators;
using VentusServer.Domain.Objects;

public static class ActionValidationHelper
{
    // Verifica si el jugador está en un estado que permite realizar acciones
    public static bool CanPerformActions(PlayerObject player)
    {
        return !player.Stats.IsDead
            && !player.Stats.IsStunned
            && !player.Stats.IsRooted
            && !player.Stats.IsSilenced
            && !player.Stats.IsChanneling;
    }

    // Verifica si el jugador está en una zona segura
    public static bool IsInSafeZone(PlayerObject player)
    {
        return player.CurrentZoneContext.IsSafeZone;
    }

    // Verifica si el target está dentro del rango de visión
    public static bool IsTargetInVisionRange(PlayerObject player, object target)
    {
        if (target is PlayerObject targetPlayer)
        {
            return ActionValidator.CalculateDistance(player.Position, targetPlayer.Position) <= player.Stats.VisionRange;
        }
        return false;
    }

    // Verifica si el target está vivo y válido
    public static bool IsTargetValid(object target)
    {
        if (target is PlayerObject targetPlayer)
        {
            return !targetPlayer.Stats.IsDead;
        }
        return false;
    }

    // Verifica si el jugador tiene recursos suficientes para realizar la acción
    public static bool HasSufficientResources(PlayerObject player, int manaCost, int energyCost)
    {
        return player.Stats.Mana >= manaCost && player.Stats.Stamina >= energyCost;
    }



}
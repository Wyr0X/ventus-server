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
        return player.CurrentMap.IsInSafeZone(player.Position);
    }

    // Verifica si el target está dentro del rango de visión
    public static bool IsTargetInVisionRange(PlayerObject player, object target)
    {
        if (target is PlayerObject targetPlayer)
        {
            return CalculateDistance(player.Position, targetPlayer.Position) <= player.Stats.VisionRange;
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
    /// <summary>
    /// Calcula la distancia entre dos posiciones.
    /// </summary>
    public static float CalculateDistance(Vec2 a, Vec2 b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
    /// <summary>
    /// Verifica si el objetivo está dentro del rango de ataque.
    /// </summary>
    public static bool IsTargetInRange(PlayerObject attacker, PlayerObject target)
    {
        var distance = CalculateDistance(attacker.Position, target.Position);
        return distance <= attacker.Stats.AttackRange;
    }

    /// <summary>
    /// Valida si el jugador puede atacar físicamente.
    /// </summary>
    public static ValidationResult CanAttack(PlayerObject attacker, PlayerObject target)
    {

        if (target == null || target.Stats.IsDead)
            return ValidationResult.Fail("El objetivo no es válido o está muerto.");

        if (!IsTargetInRange(attacker, target))
            return ValidationResult.Fail("El objetivo está fuera de rango.");

        return ValidationResult.Success();
    }

}
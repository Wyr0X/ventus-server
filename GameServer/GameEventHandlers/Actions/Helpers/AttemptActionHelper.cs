using VentusServer.Domain.Objects;

public static class AttemptActionHelper
{
    // Verifica si se puede intentar lanzar un hechizo
    public static ValidationResult CanAttemptCastSpell(PlayerObject player, SpellObject spell)
    {
        if (!ActionValidationHelper.CanPerformActions(player))
            return ValidationResult.Fail("No puedes realizar acciones en este momento.");

        if (!ActionValidationHelper.HasSufficientResources(player, spell.Model.ManaCost, 0))
            return ValidationResult.Fail("No tienes suficiente maná para lanzar este hechizo.");

        if (!player.CooldownManager.IsOffCooldown(spell.Id))
            return ValidationResult.Fail("El hechizo está en cooldown.");

        return ValidationResult.Success();
    }

    // Verifica si se puede intentar golpear
    public static ValidationResult CanAttemptHit(PlayerObject player, object target)
    {
        if (!ActionValidationHelper.CanPerformActions(player))
            return ValidationResult.Fail("No puedes realizar acciones en este momento.");

        if (!ActionValidationHelper.IsTargetValid(target))
            return ValidationResult.Fail("El objetivo no es válido.");

        if (!ActionValidationHelper.IsTargetInVisionRange(player, target))
            return ValidationResult.Fail("El objetivo está fuera de rango de visión.");

        return ValidationResult.Success();
    }

    // Verifica si se puede intentar usar un ítem
    public static ValidationResult CanAttemptUseItem(PlayerObject player, ItemObject item)
    {
        if (!ActionValidationHelper.CanPerformActions(player))
            return ValidationResult.Fail("No puedes realizar acciones en este momento.");

        if (item == null || item.Quantity <= 0)
            return ValidationResult.Fail("El ítem no es válido o no tienes suficientes cargas.");

        if (!item.CooldownManager.IsOffCooldown(item.Id))
            return ValidationResult.Fail("El ítem está en cooldown.");

        return ValidationResult.Success();
    }
}
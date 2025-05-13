using VentusServer.Domain.Objects;

public static class ExecuteActionHelper
{
    // Verifica si se puede ejecutar un hechizo
    public static ValidationResult CanExecuteCastSpell(PlayerObject player, SpellObject spell, object target)
    {
        if (!ActionValidationHelper.IsTargetValid(target))
            return ValidationResult.Fail("El objetivo no es válido.");

        if (spell.Model.RequiresLineOfSight && !ActionValidationHelper.IsTargetInVisionRange(player, target))
            return ValidationResult.Fail("No tienes línea de visión con el objetivo.");

        return ValidationResult.Success();
    }

    // Verifica si se puede ejecutar un golpe
    public static ValidationResult CanExecuteHit(PlayerObject player, object target)
    {
        if (!ActionValidationHelper.IsTargetValid(target))
            return ValidationResult.Fail("El objetivo no es válido.");

        return ValidationResult.Success();
    }

    // // Verifica si se puede ejecutar el uso de un ítem
    // public static ValidationResult CanExecuteUseItem(PlayerObject player, ItemObject item)
    // {
    //     if (item == null || item.Quantity <= 0)
    //         return ValidationResult.Fail("El ítem no es válido o no tienes suficientes cargas.");

    //     return ValidationResult.Success();
    // }
}
using VentusServer.Domain.Objects;

public class SpellActionValidator : IActionValidator
{
    private readonly SpellObject _spell;

    public SpellActionValidator(SpellObject spell)
    {
        _spell = spell;
    }

    public ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player)
    {
        // Realizar un casting seguro a SpellObject

        if (validatableObject is not SpellObject spellObject)
            return ValidationResult.Fail("El objeto no es un hechizo válido.");

        if (!ActionValidationHelper.CanPerformActions(player))
            return ValidationResult.Fail("No puedes lanzar este hechizo.");

        if (spellObject.IsOnCooldown)
            return ValidationResult.Fail("El hechizo está en cooldown.");

        if (player.Stats.Mana < spellObject.Model.ManaCost)
            return ValidationResult.Fail("No tienes suficiente maná para lanzar este hechizo.");

        return ValidationResult.Success();
    }

    public ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position)
    {
        // Realizar un casting seguro a SpellObject
        if (validatableObject is not SpellObject spellObject)
            return ValidationResult.Fail("El objeto no es un hechizo válido.");

        // Validar línea de visión, rango, etc.
        if (!ActionValidationHelper.IsTargetInVisionRange(player, position))
            return ValidationResult.Fail("El objetivo está fuera de rango o no tienes línea de visión.");

        return ValidationResult.Success();
    }
}

using Server.Logic.Validators;
using VentusServer.Domain.Objects;

public class SpellActionValidator : BaseActionValidator
{
    /// <summary>
    /// Validates whether the player can attempt to cast the spell.
    /// 
    /// First calls <see cref="ValidatePlayerState"/> to check if the player can perform general actions.
    /// If the player can't perform general actions, the result is returned immediately.
    /// 
    /// If the player can perform general actions, the method then checks if the spell is on cooldown.
    /// If the spell is on cooldown, the result is returned immediately.
    /// 
    /// If the player can perform general actions and the spell is not on cooldown, the method then calls <see cref="ActionValidator.CanCastSpell"/> to check if the player can cast the spell.
    /// The result of that validation is returned.
    /// </summary>
    /// <param name="validatableObject">The object to validate.</param>
    /// <param name="player">The player attempting to cast the spell.</param>
    /// <returns>The result of the validation.</returns>
    public override ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player)
    {
        var stateValidation = ValidatePlayerState(player);
        if (!stateValidation.IsValid)
            return stateValidation;

        // Validaciones específicas de hechizos
        if (validatableObject is not SpellObject spell)
            return stateValidation.SetValidation(false, "El objeto no es un hechizo válido.");


        if (spell.IsOnCooldown)
            return stateValidation.SetValidation(false, "El hechizo está en cooldown.");

        stateValidation = ActionValidator.CanCastSpell(player, spell);


        return stateValidation;
    }

    public override ValidationResult CanCustomExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position)
    {
        // Implementación de validación específica para ejecutar la acción
        if (validatableObject is not SpellObject spell)
            return ValidationResult.Fail("El objeto no es un hechizo válido.");

        // if (spell.Model.RequiresLineOfSight && !EnvironmentHelper.HasLineOfSight(player, position))
        //     return ValidationResult.Fail("No tienes línea de visión con el objetivo.");

        return ValidationResult.Success();
    }
}

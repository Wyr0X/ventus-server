using VentusServer.Domain.Objects;

public abstract class BaseActionValidator
{
    protected ValidationResult ValidatePlayerState(PlayerObject player)
    {
        if (!ActionValidationHelper.CanPerformActions(player))
            return ValidationResult.Fail("No puedes realizar acciones en este momento.");

        return ValidationResult.Success();
    }

    public abstract ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player);



    /// <summary>
    /// Validates whether the player can execute the action.
    /// 
    /// First calls <see cref="CanCustomAttemptAction"/> to check if the player can attempt the action.
    /// If that validation fails, the result is returned immediately.
    /// 
    /// If the player can attempt the action, the method then calls <see cref="CanCustomExecuteAction"/> to check if the player can execute the action.
    /// The result of that validation is returned.
    /// </summary>
    /// <param name="validatableObject">The object to validate.</param>
    /// <param name="player">The player attempting the action.</param>
    /// <param name="position">The position of the target of the action, if applicable.</param>
    /// <returns>The result of the validation.</returns>
    public ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position)
    {
        var validationResult = CanAttemptAction(validatableObject, player);
        if (validationResult.IsValid == false) return validationResult;

        return CanCustomExecuteAction(validatableObject, player, position);
    }
    public abstract ValidationResult CanCustomExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position);
}
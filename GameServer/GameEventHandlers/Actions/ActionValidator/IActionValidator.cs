using VentusServer.Domain.Objects;

public interface IActionValidator
{
    /// <summary>
    /// Determines if the player can attempt to perform the action.
    /// </summary>
    /// <param name="validatableObject">The object to validate.</param>
    /// <param name="player">The player attempting the action.</param>
    /// <returns>The result of the validation.</returns>
    ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player);

    /// <summary>
    /// Determines if the player can execute the action after it has been attempted.
    /// </summary>
    /// <param name="validatableObject">The object to validate.</param>
    /// <param name="player">The player attempting the action.</param>
    /// <param name="position">The position of the action's target, if applicable.</param>
    /// <returns>The result of the validation.</returns>
    ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position);
}

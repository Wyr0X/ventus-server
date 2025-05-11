using VentusServer.Domain.Objects;

public interface IActionValidator
{
    ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player);
    ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position);
}

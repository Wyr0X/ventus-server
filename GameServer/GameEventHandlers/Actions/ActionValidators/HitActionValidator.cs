using VentusServer.Domain.Objects;

public class HitActionValidator : IActionValidator
{
    private readonly PlayerObject _targetPlayer;

    public HitActionValidator(PlayerObject targetPlayer)
    {
        _targetPlayer = targetPlayer;
    }


    public ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player)
    {
        throw new NotImplementedException();
    }

    public ValidationResult CanExecuteAction(PlayerObject player, object target = null, SpellObject spell = null)
    {
        if (_targetPlayer == null)
            return ValidationResult.Fail("Objetivo no válido.");

        return ValidationResult.Success();
    }

    public ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position)
    {
        throw new NotImplementedException();
    }
}

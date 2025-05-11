using VentusServer.Domain.Objects;

public class EquipActionValidator : IActionValidator
{
    private readonly string _equipmentId;

    public EquipActionValidator(string equipmentId)
    {
        _equipmentId = equipmentId;
    }


    public ValidationResult CanAttemptAction(IValidatableObject validatableObject, PlayerObject player)
    {
        throw new NotImplementedException();
    }

    public ValidationResult CanExecuteAction(IValidatableObject validatableObject, PlayerObject player, Vec2 position)
    {
        throw new NotImplementedException();
    }
}

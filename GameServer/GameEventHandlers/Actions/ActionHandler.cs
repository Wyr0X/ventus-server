public class ActionHandler
{
    public ValidationResult HandleAction(PlayerObject player, HotbarAction action)
    {
        IActionValidator actionValidator;

        switch (action.ActionType)
        {
            case HotbarActionType.Hit:
                actionValidator = new HitActionValidator(action.Target as PlayerObject);
                break;

            case HotbarActionType.CastSpell:
                actionValidator = new SpellActionValidator(action.Spell);
                break;

            case HotbarActionType.UseItem:
                actionValidator = new UseActionValidator(action.Target as string);
                break;

            case HotbarActionType.Equip:
                actionValidator = new EquipActionValidator(action.Target as string);
                break;

            default:
                return ValidationResult.Fail("Acción desconocida.");
        }

        // Validar intento de realizar la acción
        var result = actionValidator.CanAttemptAction(player, action.Target, action.Spell);
        if (!result.IsValid)
            return result;

        // Validar si se puede ejecutar la acción
        return actionValidator.CanExecuteAction(player, action.Target, action.Spell);
    }
}

public interface IGlobalCooldown
{
    // ¿El jugador puede hacer *cualquier* acción ahora?
    bool IsAvailable(int playerId, DateTime now);

    // ¿Puede hacer esta categoría de acción (por ejemplo melee, magic)?
    bool IsAvailableForAction(int playerId, DateTime now, HotbarActionType hotbarActionType);

    // ¿Puede lanzar este hechizo en particular?
    bool IsAvailableForSpell(int playerId, DateTime now, string spellId);

    // ¿Puede usar este ítem en particular?
    bool IsAvailableForItem(int playerId, DateTime now, string itemId);
}

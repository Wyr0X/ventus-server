using VentusServer.Domain.Objects;

public static class ResourceHelper
{
    // Verifica si el jugador tiene suficientes reagentes o consumibles
    public static bool HasRequiredReagents(PlayerObject player, List<ItemRequirement> requirements)
    {
        foreach (var requirement in requirements)
        {
            var item = player.Inventory.GetItem(requirement.ItemId);
            if (item == null || item.Quantity < requirement.Quantity)
                return false;
        }
        return true;
    }

    // Verifica si el ítem tiene suficientes cargas
    public static bool HasSufficientCharges(ItemObject item)
    {
        return item != null && item.Quantity > 0;
    }

    // Verifica si el jugador tiene suficiente energía o mana
    public static bool HasSufficientEnergyOrMana(PlayerObject player, int manaCost, int energyCost)
    {
        return player.Stats.Mana >= manaCost && player.Stats.Stamina >= energyCost;
    }
}
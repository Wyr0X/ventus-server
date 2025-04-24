public class PlayerSpells
{
    public int Id { get; set; } = 0;
    public int PlayerId { get; set; }
    public PlayerSpell Spells { get; set; } = new();
}
public class PlayerSpell
{
    public string SpellId { get; set; } = string.Empty;
    public bool IsEquipped { get; set; } = false;
    public int? HotbarSlot { get; set; }
}

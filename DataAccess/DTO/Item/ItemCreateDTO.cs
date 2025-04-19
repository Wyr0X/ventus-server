// ItemCreateDTO
public class ItemCreateDTO
{
    public required string Key { get; set; }
    public required TranslatedTextDTO Name { get; set; }
    public required TranslatedTextDTO Description { get; set; }
    public required ItemType Type { get; set; }
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;
    public string? Sound { get; set; }
    public int? Damage { get; set; }
    public int? Defense { get; set; }
    public int? ManaBonus { get; set; }
    public int? StrengthBonus { get; set; }
    public int? SpeedBonus { get; set; }
    public required int MaxStack { get; set; }
    public string? IconPath { get; set; }
    public int[]? Sprite { get; set; }
    public required bool IsTradable { get; set; }
    public required bool IsDroppable { get; set; }
    public required bool IsUsable { get; set; }
}

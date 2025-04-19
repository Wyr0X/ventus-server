// ItemUpdateDTO
public class ItemUpdateDTO
{
    public TranslatedTextDTO Name { get; set; } = new TranslatedTextDTO
    {
        En = "",
        Es = ""
    };
    public TranslatedTextDTO Description { get; set; } = new TranslatedTextDTO
    {
        En = "",
        Es = ""
    };
    public ItemType? Type { get; set; }
    public ItemRarity? Rarity { get; set; }
    public string? Sound { get; set; }
    public int? Damage { get; set; }
    public int? Defense { get; set; }
    public int? ManaBonus { get; set; }
    public int? StrengthBonus { get; set; }
    public int? SpeedBonus { get; set; }
    public int? MaxStack { get; set; }
    public string? IconPath { get; set; }
    public int[]? Sprite { get; set; }
    public bool? IsTradable { get; set; }
    public bool? IsDroppable { get; set; }
    public bool? IsUsable { get; set; }
}
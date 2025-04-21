public class ItemCreateDTO
{
    public string Key { get; set; }
    public TranslatedTextDTO Name { get; set; }
    public TranslatedTextDTO Description { get; set; }

    public ItemType Type { get; set; }
    public ItemRarity Rarity { get; set; }

    public int? MaxStack { get; set; }
    public int? RequiredLevel { get; set; }
    public int? Price { get; set; }
    public int? Quantity { get; set; }

    public bool IsTradeable { get; set; }
    public bool IsDroppable { get; set; }
    public bool IsUsable { get; set; }

    public WeaponStats? WeaponData { get; set; }
    public ArmorStats? ArmorData { get; set; }
    public ConsumableEffect? ConsumableData { get; set; }

    public int[]? Sprite { get; set; }
    public string? Sound { get; set; }
    public string IconPath { get; set; }
}


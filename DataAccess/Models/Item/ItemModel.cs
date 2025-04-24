public class ItemModel
{
    public int Id { get; set; }
    public required string Key { get; set; }

    public required TranslatedTextModel Name { get; set; }
    public required TranslatedTextModel Description { get; set; }

    public ItemType Type { get; set; } // Enum: Weapon, Armor, Consumable, Book, Resource, Key, etc.
    public ItemRarity Rarity { get; set; } // Enum: Common, Rare, Epic, Legendary...

    public int? Quantity { get; set; } // Null si no es stackeable, o un número
    public int? MaxStack { get; set; } // Null si no es stackeable, o un número

    // Común a casi todos
    public int? RequiredLevel { get; set; }
    public bool IsTradable { get; set; }
    public int Price { get; set; } = 0; // Precio de venta

    // Sub-estructuras específicas por tipo
    public WeaponStats? WeaponData { get; set; }
    public ArmorStats? ArmorData { get; set; }
    public ConsumableEffect? ConsumableData { get; set; }

    public bool IsTradeable { get; set; } = false;
    public bool IsDroppable { get; set; } = false;
    public bool IsUsable { get; set; } = false;


    // Otros posibles
    public int[]? Sprite { get; set; }
    public string? Sound { get; set; }
    public string IconPath { get; set; } = ""; // Para el frontend

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

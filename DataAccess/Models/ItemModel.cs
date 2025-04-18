public class ItemModel
{
    public int Id { get; set; } // Identificador único del ítem
    public required TranslatedTextModel Name { get; set; }
    public required TranslatedTextModel Description { get; set; }

    // Tipo: Arma, Armadura, Consumible, Recurso, etc.
    public required ItemType Type { get; set; }

    // Rareza: Común, Raro, Épico, Legendario...
    public ItemRarity Rarity { get; set; } = ItemRarity.Common;

    // Stats (pueden ser null si no aplica)
    public string? Sound;
    public int? Damage { get; set; }
    public int? Defense { get; set; }
    public int? ManaBonus { get; set; }
    public int? StrengthBonus { get; set; }
    public int? SpeedBonus { get; set; }

    public required int MaxStack { get; set; } // 1 si no se stackea, 99 si es stackeable

    public string? IconPath { get; set; } // Ruta del ícono (frontend)
    public required int[] Sprite { get; set; } // Sprite en juego (opcional)
    public required string Key { get; set; } // Sprite en juego (opcional)
    public required bool IsTradable { get; set; }
    public required bool IsDroppable { get; set; }
    public required bool IsUsable { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

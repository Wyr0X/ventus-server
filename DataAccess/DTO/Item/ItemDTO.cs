// -----------------------------
// DTOs/ItemDtos.cs
// -----------------------------
namespace VentusServer.Models
{
    public class TranslatedTextDTO
    {
        public string Es { get; set; } = string.Empty;
        public string En { get; set; } = string.Empty;

        public static implicit operator TranslatedTextDTO(global::TranslatedTextDTO v)
        {
            throw new NotImplementedException();
        }
    }

    public class WeaponStatsDTO
    {
        public WeaponType WeaponType { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public float AttackSpeed { get; set; }
        public int Range { get; set; }
        public bool IsTwoHanded { get; set; }
        public int? ManaCost { get; set; }
    }

    public class ArmorStatsDTO
    {
        public ArmorSlot Slot { get; set; }
        public int Defense { get; set; }
        public int MagicResistance { get; set; }
        public int Durability { get; set; }
    }

    public class ConsumableEffectDTO
    {
        public string Type { get; set; } = string.Empty;
        public int Amount { get; set; }
        public float Duration { get; set; }
        public string? EffectName { get; set; }
    }

    public class ItemDTO
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public TranslatedTextDTO Name { get; set; } = new();
        public TranslatedTextDTO Description { get; set; } = new();
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public int? MaxStack { get; set; }
        public int? RequiredLevel { get; set; }
        public int? Price { get; set; }
        public int? Quantity { get; set; }
        public bool IsTradable { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsUsable { get; set; }
        public WeaponStatsDTO? WeaponData { get; set; }
        public ArmorStatsDTO? ArmorData { get; set; }
        public ConsumableEffectDTO? ConsumableData { get; set; }
        public int[]? Sprite { get; set; }
        public string? Sound { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ItemCreateDTO
    {
        public string Key { get; set; } = string.Empty;
        public TranslatedTextDTO Name { get; set; } = new();
        public TranslatedTextDTO Description { get; set; } = new();
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public int? MaxStack { get; set; }
        public int? RequiredLevel { get; set; }
        public int? Value { get; set; }
        public bool IsTradable { get; set; }
        public bool IsDroppable { get; set; }
        public bool IsUsable { get; set; }
        public WeaponStatsDTO? WeaponData { get; set; }
        public ArmorStatsDTO? ArmorData { get; set; }
        public ConsumableEffectDTO? ConsumableData { get; set; }
        public string? Sprite { get; set; }
        public string? Sound { get; set; }
        public string IconPath { get; set; } = string.Empty;
    }


}

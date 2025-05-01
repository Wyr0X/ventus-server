public class WeaponStats
{
    public WeaponType WeaponType { get; set; } // Enum: Sword, Bow, Staff, etc.
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public float AttackSpeed { get; set; } // Por segundo
    public int Range { get; set; }
    public bool IsTwoHanded { get; set; }
    public int? ManaCost { get; set; } // Para bastones mágicos, por ejemplo
}

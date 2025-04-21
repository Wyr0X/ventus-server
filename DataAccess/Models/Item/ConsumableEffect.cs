public class ConsumableEffect
{
    public required String Type { get; set; } // Heal, Mana, Buff, Antidote, etc.
    public int Amount { get; set; }
    public float Duration { get; set; } // En segundos, si aplica
    public string? EffectName { get; set; } // Para buffs
}


// --- Interfaces base para los distintos tipos de efectos ---

/// <summary>
/// Efecto que aplica algo a unidades (jugadores, NPCs, etc.).
/// </summary>
public interface ISpellEffect { }

public class DamageEffect : ISpellEffect
{
    public int Amount { get; set; }
    public string Element { get; set; } // fuego, hielo, etc.
}

public class HealEffect : ISpellEffect
{
    public int Amount { get; set; }
}

public class StatusEffect : ISpellEffect
{
    public string StatusId { get; set; } // "invisible", "stunned", etc.
    public float Duration { get; set; }
}

public class BuffEffect : ISpellEffect
{
    public string Attribute { get; set; } // ejemplo: "strength", "speed"
    public int Bonus { get; set; }
    public float Duration { get; set; }
}
public class TimedEffect : ISpellEffect
{
    public float Duration { get; set; }

    public TimedEffect(float duration)
    {
        Duration = duration;
    }
}

public class InvisibleEffect : TimedEffect
{
    public bool SelfOnly { get; set; } = true;

    public InvisibleEffect(float duration) : base(duration) { }

    // La lógica real de aplicación va en el sistema de efectos del juego
    // Aquí solo defines los datos del efecto
}

public class CompositeEffect : ISpellEffect
{
    public List<ISpellEffect> SubEffects { get; set; } = new();

    public CompositeEffect(params ISpellEffect[] effects)
    {
        SubEffects.AddRange(effects);
    }
}
public class SlowEffect : ISpellEffect
{
    /// <summary>
    /// Porcentaje de reducción de velocidad (ej: 50 para 50% más lento)
    /// </summary>
    public int Amount { get; set; }

    public SlowEffect() { }

    public SlowEffect(int amount, float duration)
    {
        Amount = amount;
    }
}
public class FreezeEffect : CompositeEffect
{
    public FreezeEffect(float duration, int slowAmount = 100)
        : base(
            new TimedEffect(duration), // Duración del congelamiento
            new SlowEffect { Amount = slowAmount } // 100% slow = inmovilizado
        )
    { }
}

/// <summary>
/// Efecto que invoca criaturas u objetos (por ejemplo, invocar una criatura).
/// </summary>
public interface ISummonEffect { }

public class SummonCreatureEffect : ISummonEffect
{
    public string CreatureId { get; set; }
    public int Duration { get; set; }
}

public class CreateTotemEffect : ISummonEffect
{
    public string TotemType { get; set; } // "healing", "damage", etc.
    public int Duration { get; set; }
}

public class DeployTrapEffect : ISummonEffect
{
    public string TrapType { get; set; } // "fire", "slow", etc.
    public int TriggerRadius { get; set; }
}

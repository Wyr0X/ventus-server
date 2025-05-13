
/// <summary>
/// Define cómo se seleccionan los objetivos del hechizo (área, en línea recta, objetivo único, etc.).
/// </summary>
public interface ITargetingStrategy { }
public class SingleTargetStrategy : ITargetingStrategy
{
    public TargetType ValidTarget { get; set; }
}

public class AreaOfEffectStrategy : ITargetingStrategy
{
    public float Radius { get; set; }
    public bool AffectAllies { get; set; }
    public bool AffectEnemies { get; set; }
}

public class LineTargetStrategy : ITargetingStrategy
{
    public float Length { get; set; }
    public float Width { get; set; }
}

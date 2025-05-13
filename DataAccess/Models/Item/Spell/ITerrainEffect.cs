
/// <summary>
/// Efecto que modifica el terreno (crea fuego, niebla, etc.).
/// </summary>
public interface ITerrainEffect { }

public class CreateFireZone : ITerrainEffect
{
    public int Duration { get; set; }
    public int DamagePerSecond { get; set; }
    public float Radius { get; set; }
}

public class CreateFog : ITerrainEffect
{
    public int Duration { get; set; }
    public float VisibilityReduction { get; set; }
}

public class SlipperyGroundEffect : ITerrainEffect
{
    public int Duration { get; set; }
    public float SlipChance { get; set; }
}

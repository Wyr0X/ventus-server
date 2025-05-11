public class ZoneContext
{
    public string WorldId { get; set; }                // ID del mundo
    public string MapId { get; set; }                  // ID del mapa
    public string ZoneId { get; set; }                 // ID de la zona

    public bool IsSafeZone { get; set; }
    public bool AllowsCombat { get; set; }
    public bool IsDungeonArea { get; set; }
    public bool IsNoSpellZone { get; set; }

    public int VisionRange { get; set; }

    private readonly TerrainType[,] _terrainMap;

    public int Width { get; }
    public int Height { get; }

    public ZoneContext(
        string worldId,
        string mapId,
        string zoneId,
        int width,
        int height,
        TerrainType[,] terrainMap,
        bool isSafeZone = false,
        bool allowsCombat = true,
        bool isDungeonArea = false,
        bool isNoSpellZone = false,
        int visionRange = 10)
    {
        WorldId = worldId;
        MapId = mapId;
        ZoneId = zoneId;

        Width = width;
        Height = height;
        _terrainMap = terrainMap;

        IsSafeZone = isSafeZone;
        AllowsCombat = allowsCombat;
        IsDungeonArea = isDungeonArea;
        IsNoSpellZone = isNoSpellZone;
        VisionRange = visionRange;
    }

    public TerrainType GetTerrainAt(float x, float y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return TerrainType.Impassable; // fuera de los límites

        return _terrainMap[(int)x, (int)y];
    }

    public bool IsWater(float x, float y) => GetTerrainAt(x, y) == TerrainType.Water;
    public bool IsLava(float x, float y) => GetTerrainAt(x, y) == TerrainType.Lava;
    public bool IsImpassable(float x, float y) => GetTerrainAt(x, y) == TerrainType.Impassable;
}

public enum TerrainType
{
    Normal,
    Water,
    Lava,
    Impassable
}

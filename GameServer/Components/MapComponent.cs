
public class MapComponent : Component
{
    int MapId { get; }
    int WorldId { get; }
    

    
    public MapComponent(int mapId, int worldId)
    {
        MapId = mapId;
        WorldId = worldId;
    }
    public int GetId(){
        return MapId;
    }
    public int GetWorldId(){
        return WorldId;
    }
}

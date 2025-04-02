
public class WorldComponent : Component
{
    int WorldId { get; }

    
    public WorldComponent(int worldId)
    {
        WorldId = worldId;
    }
    public int GetWorldId(){
        return WorldId;
    }
}

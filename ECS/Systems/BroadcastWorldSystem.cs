public class BroadcastWorldSystem : ISystem
{
    public BroadcastWorldSystem(World world) {
        
    }

    public void Run(EntityManager entities)
    {
        foreach (var (component, entity) in entities.Get(typeof(FollowComponent)))
        {
            
        }
    }
}

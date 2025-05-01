public class FollowEntitySystem : ISystem
{
    public void Run(EntityManager entities)
    {
        foreach (var (component, entity) in entities.Get(typeof(FollowComponent)))
        {
            FollowComponent follow = (FollowComponent)component;
            Position? entityPos = (Position?)entity.Get(typeof(Position));
            Entity entityOfFollow = follow.Entity;
            Position? positionComponent = (Position?)entityOfFollow.Get(typeof(Position));
            if (entityPos == null || positionComponent == null) continue;
 
            entityPos.Set(positionComponent.X, positionComponent.Y);
        }
    }
}
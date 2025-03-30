public class FollowComponent : IComponent
{
    public Entity Entity { get; set; }

    public FollowComponent(Entity entityArg)
    {
        Entity = entityArg;
    }
}

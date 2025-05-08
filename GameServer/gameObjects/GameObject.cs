public abstract class GameObject
{
    public int Id { get; set; }
    public Vec2 Position { get; set; }
    public bool Show { get; set; } = true;

    protected GameObject(int id, Vec2 position)
    {
        Id = id;
        Position = position;
    }

}

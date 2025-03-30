
public class Position : Vec2, IComponent
{
    public float X { get; set; }
    public float Y { get; set; }

    public Position(float x = 0, float y = 0)
    {
        X = x;
        Y = y;
    }

    // Puedes agregar métodos específicos de Position si es necesario
    public override string ToString() => $"({X}, {Y})";

    internal void Set(float x, float y)
    {
        X = x;
        Y = y;
    }
}

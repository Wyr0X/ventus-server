public abstract class GameObject
{
    /// <summary>
    /// Identificador único de este objeto en el juego.
    /// Puede ser string, int, GUID u otro tipo.
    /// </summary>
    public object Id { get; protected set; }

    /// <summary>
    /// Posición en el mundo.
    /// </summary>
    public Vec2 Position { get; init; }

    /// <summary>
    /// Constructor base que ya deja inicializado el Id requerido.
    /// </summary>
    protected GameObject(object id, Vec2 position)
    {
        Id = id;
        Position = position;
    }
}

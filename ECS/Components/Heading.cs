// Ahora la clase Heading se convierte en un struct.
public class Heading: IComponent
{
    public DirectionComponent Direction { get; }

    // Constructor que inicializa el campo Direction
    public Heading(DirectionComponent direction)
    {
        Direction = direction;
    }
}

public class UpdateMovableSystem : ISystem
{
    public void Run(EntityManager entityManager)
    {
        // Obtener todas las entidades con el componente Movable
        var movableEntities = entityManager.Get(typeof(Movable));

        foreach (var (component, entity) in movableEntities)
        {
            Movable movable = (Movable)component;
            if (movable.Moving) return;

            Position? position = (Position?)entity.Get(typeof(Position));
            // Calcular el vector de dirección y actualizar la posición
            // Ahora usando 'Direction' para obtener el 'Vec2'
            Heading? heading = (Heading?)entity.Get(typeof(Heading));
            if (position ==null || heading == null) return;
            Vec2 directionVec = DirectionUtil.ToVec2(heading.Direction.directionEnum); // Usamos el método ToVec2() del struct 'Direction'
            Vec2 movement = Vec2.Scale(directionVec, movable.Speed);

            // Actualizar la posición sumando el movimiento calculado
            position.X = movement.X;
            position.Y = movement.Y;

        }
    }
}


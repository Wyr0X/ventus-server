namespace VentusServer.Domain.Objects
{
    public class Character : GameObject
    {
        public const int TIME_STEP = 1000 / 60; // 60fps
        public new int Id { get; set; }
        public string Name { get; set; }
        public Direction Heading { get; set; } = Direction.Down;
        public float Speed { get; private set; } = (150f / 1000f) * TIME_STEP;

        public Character(int id, Vec2 position, string name)
            : base(id, position)
        {
            Name = name;
        }


        public void Move(Direction direction, int distance)

        {
            if (direction == Direction.None) return;

            Vec2 newPosition = Position;

            switch (direction)
            {
                case Direction.Up: newPosition.Y -= distance; break;
                case Direction.Down: newPosition.Y += distance; break;
                case Direction.Left: newPosition.X -= distance; break;
                case Direction.Right: newPosition.X += distance; break;
                default: return;
            }
        }

        public void ChangeDirection(Direction direction)
        {

        }


    }
}
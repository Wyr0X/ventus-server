using System;

public enum DirectionKeyEnum
{
    Down = 0,
    Left = 1,
    Up = 2,
    Right = 3
}

public static class DirectionUtil
{
    public static DirectionKeyEnum Opposite(DirectionKeyEnum directionEnum)
    {
       return (DirectionKeyEnum)(((int)directionEnum + 2) % 4);
    }

    public static Vec2 ToVec2(DirectionKeyEnum direction)
    {
        return direction switch
        {
            DirectionKeyEnum.Up => Vec2.Up,
            DirectionKeyEnum.Down => Vec2.Down,
            DirectionKeyEnum.Left => Vec2.Left,
            DirectionKeyEnum.Right => Vec2.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), "Invalid direction")
        };
    }
}

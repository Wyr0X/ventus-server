using System;

public class Vec2: Component
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vec2(float x = 0, float y = 0)
    {
        X = x;
        Y = y;
    }

    public Vec2 Set(Vec2 v)
    {
        X = v.X;
        Y = v.Y;
        return this;
    }

    public Vec2 Set(float x, float y)
    {
        X = x;
        Y = y;
        return this;
    }

    public Vec2 Add(Vec2 v)
    {
        X += v.X;
        Y += v.Y;
        return this;
    }

    public Vec2 Sub(Vec2 v)
    {
        X -= v.X;
        Y -= v.Y;
        return this;
    }

    public Vec2 Mult(Vec2 v)
    {
        X *= v.X;
        Y *= v.Y;
        return this;
    }

    public Vec2 Div(Vec2 v)
    {
        X /= v.X;
        Y /= v.Y;
        return this;
    }

    public Vec2 Scale(float s)
    {
        X *= s;
        Y *= s;
        return this;
    }

    public Vec2 Neg()
    {
        X = -X;
        Y = -Y;
        return this;
    }

    public float Dot(Vec2 v)
    {
        return X * v.X + Y * v.Y;
    }

    public float Length()
    {
        return (float)Math.Sqrt(X * X + Y * Y);
    }

    public Vec2 Normalize()
    {
        float len = Length();
        if (len != 0)
        {
            X /= len;
            Y /= len;
        }
        return this;
    }

    public static Vec2 Add(Vec2 v1, Vec2 v2)
    {
        return new Vec2(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vec2 Sub(Vec2 v1, Vec2 v2)
    {
        return new Vec2(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static Vec2 Scale(Vec2 v, float s)
    {
        return new Vec2(v.X * s, v.Y * s);
    }

    public static Vec2 Normalize(Vec2 v)
    {
        float len = v.Length();
        return len == 0 ? new Vec2(0, 0) : new Vec2(v.X / len, v.Y / len);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    public static readonly Vec2 Null = new Vec2(float.NaN, float.NaN);
    public static readonly Vec2 Zero = new Vec2(0, 0);
    public static readonly Vec2 One = new Vec2(1, 1);
    public static readonly Vec2 Left = new Vec2(-1, 0);
    public static readonly Vec2 Up = new Vec2(0, -1);
    public static readonly Vec2 Right = new Vec2(1, 0);
    public static readonly Vec2 Down = new Vec2(0, 1);
}

using System;

public class Movable : Component
{
    public bool Moving { get; set; }
    public float Speed { get; private set; } // Velocidad en píxeles por milisegundo

    public Movable(float speed = 150)
    {
        SetSpeed(speed);
    }

    public void SetSpeed(float speed)
    {
        Speed = (float)((speed / 1000) * Engine.TIME_STEP);
    }
}

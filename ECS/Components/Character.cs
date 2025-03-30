using System;
using System.Collections.Generic;

public class Character : IComponent
{
    private double startTime = 0;
    private DirectionComponent _direction;

    public Character(DirectionComponent direction)
    {
        _direction = direction;
    }

    public void Start(double time)
    {
      
        startTime = time;
    }

    public void Stop()
    {
        
        startTime = 0;
    }

    public void ChangeDirection(DirectionComponent direction)
    {
        _direction = direction;
    }

    internal void Start(object time)
    {
        throw new NotImplementedException();
    }
}
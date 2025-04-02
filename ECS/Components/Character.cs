using System;
using System.Collections.Generic;

public class Character : Component
{
    private double startTime = 0;
    public int PlayerId { get; set; }
    public string UserId { get; set; }

    public int CurrentMapId { get; set; }
    public int CurrentWorldId { get; set; }
    private DirectionComponent _direction;

    public Character( string userId, int playerId, int currentMapId, int currentWorldId)
    {
        UserId = userId;
        PlayerId = playerId;
        CurrentMapId = currentMapId;
        CurrentWorldId = currentWorldId;
    }
    
    public int GetCurrentWorldId(){
        return CurrentWorldId;
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
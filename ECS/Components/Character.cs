using System;
using System.Collections.Generic;

public class Character : Component
{
    private double startTime = 0;
    public int PlayerId { get; set; }
    public Guid AccountId { get; set; }

    public int CurrentMapId { get; set; }
    public int CurrentWorldId { get; set; }
    private DirectionComponent _direction;

    public Character( Guid accountId, int playerId, int currentMapId, int currentWorldId)
    {
        AccountId = accountId;
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
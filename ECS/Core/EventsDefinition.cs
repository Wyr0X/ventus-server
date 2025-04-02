
using static GameUtils;

public class GameEvent
{
    string UserId { get; set; }


    int PlayerId { get; set; }
    public GameEvent(string userId, int playerId)
    {
        UserId = userId;
        PlayerId = playerId;
    }

    public string GetUserId()
    {
        return UserId;
    }
}


public class InputsKeyEvent : GameEvent
{
    List<KeyEnum> Keys { get; set; }

    public InputsKeyEvent(string userId, int playerId, List<KeyEnum> keys) : base(userId, playerId)
    {
        Keys = keys;
    }
}
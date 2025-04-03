
using static GameUtils;

public class GameEvent
{
    Guid AccountId { get; set; }


    int PlayerId { get; set; }
    public GameEvent(Guid accountId, int playerId)
    {
        AccountId = accountId;
        PlayerId = playerId;
    }

    public Guid GetUserId()
    {
        return AccountId;
    }
}


public class InputsKeyEvent : GameEvent
{
    List<KeyEnum> Keys { get; set; }

    public InputsKeyEvent(Guid accountId, int playerId, List<KeyEnum> keys) : base(accountId, playerId)
    {
        Keys = keys;
    }
}
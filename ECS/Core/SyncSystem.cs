
using Protos.Game.World;

public class SyncSystem
{
    private EntityManager entityManager;
    private Lazy<MessageSender> messageSender;
    public SyncSystem(EntityManager entityManager, Lazy<MessageSender> messageSender)
    {
        this.entityManager = entityManager;
        this.messageSender = messageSender;
    }

    public void UpdatePosition(Guid accountId, int playerId, float x, float y)
    {

       messageSender.Value.SendPlayerPosition(accountId, playerId, x, y);

    }
    public void SpawnPlayer(Guid accountId, int playerId, float x, float y)
    {


       messageSender.Value.SpawnPlayer(accountId, playerId, x, y);

    }
    public void SendWorlState(List<Guid> accountsIds, WorldStateUpdate worldStateUpdate){
        messageSender.Value.SendWorlState(accountsIds, worldStateUpdate ); 

    }
}

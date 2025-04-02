using ProtosCommon;

public class SyncSystem
{
    private EntityManager entityManager;
    private Lazy<MessageSender> messageSender;
    public SyncSystem(EntityManager entityManager, Lazy<MessageSender> messageSender)
    {
        this.entityManager = entityManager;
        this.messageSender = messageSender;
    }

    public void UpdatePosition(string userId, int playerId, float x, float y)
    {

       messageSender.Value.SendPlayerPosition(userId, playerId, x, y);

    }
    public void SpawnPlayer(string userId, int playerId, float x, float y)
    {


       messageSender.Value.SpawnPlayer(userId, playerId, x, y);

    }
}

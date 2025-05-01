using VentusServer.DataAccess.Postgres;

public class PersistenceManager
{
    public PlayerPersistenceSystem _playerPersistenceSystem { get; }

    public PersistenceManager(PlayerPersistenceSystem playerPersistenceSystem)
    {
        _playerPersistenceSystem = playerPersistenceSystem;
    }

    public void updatePlayer(){
        // _playerPersistenceSystem.SavePlayerData();
    }
}

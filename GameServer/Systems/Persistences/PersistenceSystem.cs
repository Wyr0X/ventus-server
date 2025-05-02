using VentusServer.DataAccess.Postgres;
using VentusServer.Services;

public class PersistenceManager
{
    private readonly PlayerService _playerService;

    public PersistenceManager(PlayerService playerService)
    {
        _playerService = playerService;
    }

}

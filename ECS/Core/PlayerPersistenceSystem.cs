using VentusServer.Services;

public class PlayerPersistenceSystem
{
    private readonly PlayerService _playerService;
    private readonly PlayerLocationService _playerLocationService;



    public PlayerPersistenceSystem(PlayerService playerService, PlayerLocationService playerLocationService)
    {
        _playerService = playerService;
        _playerLocationService = playerLocationService;
    }

    public async void SavePlayerData(PlayerModel player, PlayerLocation playerLocationModel)
    {
        await _playerLocationService.SavePlayerLocationAsync(playerLocationModel);
       await _playerService.SavePlayerAsync(player);



    }
}

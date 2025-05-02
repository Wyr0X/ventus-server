using Game.Models;
using VentusServer.Services;

public interface IGameServiceMediator
{
    Task<WorldModel?> GetWorldInfo(int worldId);
}

public class GameServiceMediator : IGameServiceMediator
{
    private readonly PlayerService _playerService;
    private readonly AccountService _accountService;
    private readonly WorldService _worldService;

    public GameServiceMediator(PlayerService playerService, AccountService accountService,
    WorldService worldService)
    {
        _playerService = playerService;
        _accountService = accountService;
        _worldService = worldService;
    }

    public async Task<WorldModel?> GetWorldInfo(int worldId)
    {
        return await _worldService.GetWorldByIdAsync(worldId);
    }
}

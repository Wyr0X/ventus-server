using Game.Models;
using VentusServer.Services;

public interface IGameServiceMediator
{
    Task<WorldModel?> GetWorldInfo(int worldId);
}

public class GameServiceMediator : IGameServiceMediator
{
    private readonly PlayerService _playerService;
    private readonly IAccountService _IAccountService;
    private readonly WorldService _worldService;

    public GameServiceMediator(PlayerService playerService, IAccountService IAccountService,
    WorldService worldService)
    {
        _playerService = playerService;
        _IAccountService = IAccountService;
        _worldService = worldService;
    }

    public async Task<WorldModel?> GetWorldInfo(int worldId)
    {
        return await _worldService.GetWorldByIdAsync(worldId);
    }
}

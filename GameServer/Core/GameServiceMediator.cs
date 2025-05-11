using Game.Models;
using VentusServer.Services;

public interface IGameServiceMediator
{
    Task<WorldModel?> GetWorldInfo(int worldId);
}

public class GameServiceMediator : IGameServiceMediator
{
    private readonly PlayerService _playerService;
    private readonly SpellService _spellService;
    private readonly IAccountService _IAccountService;
    private readonly WorldService _worldService;

    public GameServiceMediator(PlayerService playerService, IAccountService IAccountService,
    WorldService worldService, SpellService spellService)
    {
        _playerService = playerService;
        _IAccountService = IAccountService;
        _worldService = worldService;
        _spellService = spellService;
    }
    public async Task<IEnumerable<SpellModel>> GetSpells()
    {
        return await _spellService.GetAllSpellsAsync();
    }
    public async Task<WorldModel?> GetWorldInfo(int worldId)
    {
        return await _worldService.GetWorldByIdAsync(worldId);
    }
}

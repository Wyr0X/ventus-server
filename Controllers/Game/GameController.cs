// -----------------------------
// Controllers/GameController.cs
// -----------------------------
using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.Models;
using VentusServer.Services;

[ApiController]
[Route("game")]
[JwtAuthRequired]
public class GameController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly PlayerService _playerService;

    public GameController(AccountService accountService, PlayerService playerService)
    {
        _accountService = accountService;
        _playerService = playerService;
    }

    [HttpPost("play")]
    public async Task<IActionResult> Play([FromBody] PlayRequest request)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
        if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
        {
            Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
            Console.ResetColor();
            return BadRequest("Error al obtener la cuenta.");
        }

        Console.ForegroundColor = ConsoleColor.Blue;

        var account = await _accountService.GetOrCreateAccountInCacheAsync(accountId);
        if (account == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            return Unauthorized("Cuenta no encontrada.");
        }
        if (account == null)
            return Unauthorized("Cuenta no encontrada.");

        var player = await _playerService.GetPlayerByIdAsync(request.PlayerId, new PlayerModuleOptions
        {
            IncludeStats = true,
            IncludeInventory = true,
            IncludeLocation = true,
        });
        if (player == null || player.AccountId != account.AccountId)
            return NotFound("Personaje no válido o no pertenece a tu cuenta.");



        return Ok(new
        {
            player
        });
    }
}
// -----------------------------
// Requests/PlayRequest.cs
// -----------------------------

public class PlayRequest
{
    public int PlayerId { get; set; }
}

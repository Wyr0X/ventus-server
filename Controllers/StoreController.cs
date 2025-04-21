using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Services;

[ApiController]
[Route("store")]
[JwtAuthRequired]
public class StoreController : ControllerBase
{
    private readonly StoreService _storeService;
    private readonly IPlayerDAO _playerDAO;
    private readonly PlayerInventoryService _inventoryService;

    public StoreController(StoreService storeService, PlayerInventoryService inventoryService,
     IPlayerDAO playerDAO)
    {
        _storeService = storeService;
        _inventoryService = inventoryService;
        _playerDAO = playerDAO;
    }

    [HttpPost("buy")]
    public async Task<ActionResult> BuyItem([FromBody] BuyItemRequest request)
    {
        var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
        if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
        {
            return BadRequest("Error al obtener la cuenta.");
        }

        var players = await _playerDAO.GetPlayersByAccountIdAsync(accountId);
        var player = players.Find((player) => player.Id == request.PlayerId);
        if (player != null)
        {
            var result = await _storeService.BuyItemAsync(player,
            request.ItemId, request.Quantity);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result);

        }
        return BadRequest("Usuario no encontrado");


    }


}

public class BuyItemRequest
{
    public int PlayerId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; } = 1;
}

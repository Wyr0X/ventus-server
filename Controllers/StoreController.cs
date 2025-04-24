
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



    [HttpPost("buy-cart")]

    public async Task<ActionResult> BuyCart([FromBody] BuyCartRequest request)

    {
        LoggerUtil.Log(LoggerUtil.LogTag.StoreController, $"BuyCart iniciado. PlayerId: {request.PlayerId}, Items: {request.cartItems?.Count ?? 0}, Spells: {request.cartSpells?.Count ?? 0}");
        var accountIdParam = HttpContext.Items["AccountId"]?.ToString();

        if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))

        {
            LoggerUtil.Log(LoggerUtil.LogTag.StoreController, "Error al obtener la cuenta del contexto.", isError: true);
            return BadRequest("Error al obtener la cuenta.");

        }


        LoggerUtil.Log(LoggerUtil.LogTag.StoreController, $"AccountId recibido: {accountId}");

        var players = await _playerDAO.GetPlayersByAccountIdAsync(accountId);

        var player = players.Find((p) => p.Id == request.PlayerId);

        if (player == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.StoreController, $"Jugador con ID {request.PlayerId} no encontrado para AccountId {accountId}", isError: true);
            return BadRequest("Jugador no encontrado.");

        }

        var result = await _storeService.BuyCartAsync(player, request.cartItems ?? new List<CartItem>(), request.cartSpells ?? new List<CartSpell>());



        if (!result.Success)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.StoreController, $"Compra fallida: {result.ErrorMessage}", isError: true);
            return BadRequest(result.ErrorMessage);
        }

        LoggerUtil.Log(LoggerUtil.LogTag.StoreController, $"Compra exitosa para PlayerId: {player.Id}");

        return Ok(result);

    }







}



public class BuyItemRequest

{

    public int PlayerId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; } = 1;

}
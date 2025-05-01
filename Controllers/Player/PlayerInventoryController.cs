using Microsoft.AspNetCore.Mvc;
using VentusServer.Services;
using VentusServer.Auth;
using System;
using System.Threading.Tasks;
using Game.DTOs;

namespace VentusServer.Controllers
{
    [Route("player-inventory")]
    [ApiController]
    public class PlayerInventoryController : ControllerBase
    {
        private readonly PlayerInventoryService _playerInventoryService;

        public PlayerInventoryController(PlayerInventoryService playerInventoryService)
        {
            _playerInventoryService = playerInventoryService;
        }

        // Mover un ítem de un slot a otro
        [HttpPost("move-item")]
        [JwtAuthRequired]
        public async Task<IActionResult> MoveItem([FromBody] MoveItemRequestDTO moveItemRequest)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerInventoryController] Intentando mover ítem del slot {moveItemRequest.FromSlot} al {moveItemRequest.ToSlot} para el jugador {moveItemRequest.PlayerId}");
                Console.ResetColor();

                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
                    Console.ResetColor();
                    return BadRequest("Error al obtener la cuenta.");
                }

                var success = await _playerInventoryService.MoveItemAsync(moveItemRequest.PlayerId, moveItemRequest.FromSlot, moveItemRequest.ToSlot);
                if (!success)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] No se pudo mover el ítem para el jugador {moveItemRequest.PlayerId}");
                    Console.ResetColor();
                    return BadRequest("No se pudo mover el ítem.");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] Ítem movido exitosamente de {moveItemRequest.FromSlot} a {moveItemRequest.ToSlot} para el jugador {moveItemRequest.PlayerId}");
                Console.ResetColor();

                return Ok(new { Message = "Ítem movido exitosamente" });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Excepción al mover el ítem: {ex.Message}");
                Console.ResetColor();
                return BadRequest($"Error al mover el ítem: {ex.Message}");
            }
        }
    }

    public class MoveItemRequestDTO
    {
        public required int PlayerId { get; set; }
        public required int FromSlot { get; set; }
        public required int ToSlot { get; set; }
    }
}

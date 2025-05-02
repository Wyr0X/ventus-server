using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.Models;
using VentusServer.Services;

namespace VentusServer.Controllers.Admin
{
    [ApiController]
    [Route("admin/players")]
    [JwtAuthRequired]
    [RequirePermission]
    public class AdminPlayersController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly IAccountService _IAccountService;  // Inyectamos el servicio de cuentas

        public AdminPlayersController(PlayerService playerService, IAccountService IAccountService)
        {
            _playerService = playerService;
            _IAccountService = IAccountService;  // Inicializamos el servicio de cuentas
        }

        // Obtener todos los jugadores
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayers();
            var playerDTOs = new List<PlayerDTO>();

            foreach (var p in players)
            {
                // Buscar la cuenta asociada a este jugador
                var account = await _playerService.GetAccountByPlayerIdAsync(p.Id);
                if (account == null)
                {
                    return NotFound("Cuenta no encontrada.");

                }
                playerDTOs.Add(new PlayerDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Gender = p.Gender.ToString(),
                    Race = p.Race.ToString(),
                    Level = p.Level,
                    Class = p.Class.ToString(),
                    CreatedAt = p.CreatedAt,
                    Status = p.Status,
                    AccountName = account.AccountName // Añadimos el accountId si existe
                });
            }

            return Ok(playerDTOs);
        }

        // Obtener un jugador por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
                return NotFound("Jugador no encontrado.");

            // Buscar la cuenta asociada a este jugador
            var account = await _playerService.GetAccountByPlayerIdAsync(player.Id);
            if (account == null)
            {
                return NotFound("Cuenta no encontrada.");

            }
            var playerDTO = new PlayerDTO
            {
                Id = player.Id,
                Name = player.Name,
                Gender = player.Gender.ToString(),
                Race = player.Race.ToString(),
                Level = player.Level,
                Class = player.Class.ToString(),
                CreatedAt = player.CreatedAt,
                Status = player.Status,
                AccountName = account.AccountName // Añadimos el accountId
            };

            return Ok(playerDTO);
        }

        // Eliminar un jugador
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var success = await _playerService.DeletePlayerAsync(id);
            if (!success)
                return NotFound("Jugador no encontrado o no pudo eliminarse.");

            return Ok("Jugador eliminado.");
        }

        // Actualizar jugador (uso de PlayerUpdateDTO)
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdatePlayer(int id, [FromBody] PlayerUpdateDTO updateRequest)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);
            if (player == null)
                return NotFound("Jugador no encontrado.");

            // Actualizar los campos que se proporcionan en el DTO
            if (!string.IsNullOrEmpty(updateRequest.Name))
                player.Name = updateRequest.Name;

            if (updateRequest.Level.HasValue)
                player.Level = updateRequest.Level.Value;



            // Actualizar Gender y Race
            player.Class = updateRequest.Class;
            player.Gender = updateRequest.Gender;
            player.Race = updateRequest.Race;

            await _playerService.SavePlayerAsync(player);

            return Ok("Jugador actualizado.");
        }
    }
}

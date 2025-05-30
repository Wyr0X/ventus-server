using Microsoft.AspNetCore.Mvc;
using VentusServer.Services;
using VentusServer.DataAccess.Postgres;
using Game.Models;
using Game.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Auth;
using VentusServer.DataAccess;

namespace VentusServer.Controllers
{
    [Route("player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly IAccountDAO _accountDAO;

        public PlayerController(PlayerService playerService, PlayerLocationService playerLocationService, IAccountDAO accountDAO)
        {
            _playerService = playerService;
            _playerLocationService = playerLocationService;
            _accountDAO = accountDAO;
        }

        // Crear un nuevo personaje
        [HttpPost("create-player")]
        [JwtAuthRequired]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest createPlayerRequest)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Intentando crear un personaje con nombre: {createPlayerRequest.Name}");
                Console.ResetColor();

                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
                    Console.ResetColor();
                    return BadRequest("Error al obtener la cuenta.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Buscando cuenta con ID: {accountId}");
                Console.ResetColor();

                var account = await _accountDAO.GetAccountByAccountIdAsync(accountId);
                if (account == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Cuenta con ID {accountId} no encontrada.");
                    Console.ResetColor();
                    return Unauthorized("Cuenta no encontrada.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Creando personaje para la cuenta {accountId} {account.AccountId}: {createPlayerRequest.Name}");
                Console.ResetColor();
                var createPlayerDTO = new CreatePlayerDTO
                {
                    Name = createPlayerRequest.Name,
                    Gender = (Gender)createPlayerRequest.Gender, // Conversión de int a enum
                    Race = (Race)createPlayerRequest.Race,         // Conversión de int a enum
                    Class = (CharacterClass)createPlayerRequest.Class
                };

                var newPlayer = await _playerService.CreatePlayer(accountId, createPlayerDTO);
                if (newPlayer == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Error al crear el personaje {createPlayerRequest.Name}.");
                    Console.ResetColor();
                    return BadRequest("Error al crear el personaje.");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] Personaje {newPlayer.Name} creado exitosamente para la cuenta {accountId}.");
                Console.ResetColor();

                return Ok(new { Message = "Personaje creado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Excepción al crear el personaje: {ex.Message}");
                Console.ResetColor();
                return BadRequest($"Error al crear el personaje: {ex.Message}");
            }
        }

        // Obtener todos los personajes del usuario autenticado
        [HttpGet("get-players")]
        [JwtAuthRequired]
        public async Task<IActionResult> GetPlayers()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("[PlayerController] Solicitando lista de personajes.");
                Console.ResetColor();

                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[PlayerController] AccountId {accountIdParam}");
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
                    Console.ResetColor();
                    return BadRequest("Error al obtener la cuenta.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Buscando cuenta con ID: {accountId}");
                Console.ResetColor();

                var account = await _accountDAO.GetAccountByAccountIdAsync(accountId);
                if (account == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Cuenta con ID {accountId} no encontrada.");
                    Console.ResetColor();
                    return Unauthorized("Cuenta no encontrada.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Obteniendo personajes para la cuenta {accountId}");
                Console.ResetColor();

                var players = await _playerService.GetPlayersByAccountId(account.AccountId, new PlayerModuleOptions
                {
                    IncludeInventory = true,
                    IncludeLocation = true,
                    IncludeStats = true,
                    IncludeSpells = true,
                });
                var playerDTOs = new List<PlayerDTO>();
                Console.WriteLine($"[PlayerController] Players obtenido {players.Count}");

                if (players != null)
                {
                    return Ok(new GetPlayersResponseDTO { Players = players });
                }
                return Ok(new GetPlayersResponseDTO { Players = [] });



            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Excepción al obtener los personajes: {ex.Message}");
                Console.ResetColor();
                return BadRequest($"Error al obtener los personajes: {ex.Message}");
            }

        }
        [HttpDelete("delete-player/{playerId}")]
        [JwtAuthRequired]
        public async Task<IActionResult> DeletePlayer(int playerId)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Intentando eliminar el personaje con ID: {playerId}");
                Console.ResetColor();

                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
                    Console.ResetColor();
                    return BadRequest("Error al obtener la cuenta.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Verificando cuenta con ID: {accountId}");
                Console.ResetColor();

                var account = await _accountDAO.GetAccountByAccountIdAsync(accountId);
                if (account == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Cuenta con ID {accountId} no encontrada.");
                    Console.ResetColor();
                    return Unauthorized("Cuenta no encontrada.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Buscando personaje con ID: {playerId} en la cuenta {accountId}");
                Console.ResetColor();

                var player = await _playerService.GetPlayerByIdAsync(playerId);
                if (player == null || player.AccountId != account.AccountId)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Personaje con ID {playerId} no encontrado o no pertenece a la cuenta.");
                    Console.ResetColor();
                    return NotFound("Personaje no encontrado o no pertenece a tu cuenta.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[PlayerController] Eliminando personaje con ID: {playerId}");
                Console.ResetColor();

                bool deleted = await _playerService.DeletePlayerAsync(playerId);
                if (!deleted)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARNING] Error al eliminar el personaje con ID {playerId}");
                    Console.ResetColor();
                    return BadRequest("No se pudo eliminar el personaje.");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] Personaje con ID {playerId} eliminado exitosamente.");
                Console.ResetColor();

                return Ok(new { Message = "Personaje eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Excepción al eliminar el personaje: {ex.Message}");
                Console.ResetColor();
                return BadRequest($"Error al eliminar el personaje: {ex.Message}");
            }
        }

    }

    public class CreatePlayerRequest
    {
        public required string Name { get; set; }
        public required int Gender { get; set; }
        public required int Race { get; set; }
        public required int Class { get; set; }
    }
}

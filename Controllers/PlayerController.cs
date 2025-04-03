using Microsoft.AspNetCore.Mvc;
using VentusServer.Services;
using VentusServer.DataAccess.Postgres;
using Game.Models;
using Game.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Auth;

namespace VentusServer.Controllers
{
    [Route("player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly PostgresAccountDAO _accountDAO;

        public PlayerController(PlayerService playerService, PlayerLocationService playerLocationService, PostgresAccountDAO accountDAO)
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
                Console.WriteLine($"[INFO] Intentando crear un personaje con nombre: {createPlayerRequest.Name}");
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
                Console.WriteLine($"[INFO] Buscando cuenta con ID: {accountId}");
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
                Console.WriteLine($"[INFO] Creando personaje para la cuenta {accountId}: {createPlayerRequest.Name}");
                Console.ResetColor();

                var newPlayer = await _playerService.CreatePlayer(account.AccountId, createPlayerRequest.Name, createPlayerRequest.Gender, createPlayerRequest.Race, createPlayerRequest.Class);
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

                return Ok(new { Message = "Personaje creado exitosamente", Player = newPlayer });
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
                Console.WriteLine("[INFO] Solicitando lista de personajes.");
                Console.ResetColor();

                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[INFO] AccountId {accountIdParam}");
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    Console.WriteLine("[WARNING] Error al obtener la cuenta. Token inválido o mal formateado.");
                    Console.ResetColor();
                    return BadRequest("Error al obtener la cuenta.");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[INFO] Buscando cuenta con ID: {accountId}");
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
                Console.WriteLine($"[INFO] Obteniendo personajes para la cuenta {accountId}");
                Console.ResetColor();

                var players = await _playerService.GetPlayersByAccountId(account.AccountId);
                var playerDTOs = new List<PlayerDTO>();

                foreach (var player in players)
                {
                    var playerLocation = await _playerLocationService.GetPlayerLocationAsync(player.Id);
                    if (playerLocation == null) continue;

                    playerDTOs.Add(new PlayerDTO
                    {
                        Id = player.Id,
                        AccountId = player.AccountId,
                        Name = player.Name,
                        Gender = player.Gender,
                        Race = player.Race,
                        Level = player.Level,
                        Class = player.Class,
                        CreatedAt = player.CreatedAt,
                        LastLogin = player.LastLogin,
                        Status = player.Status,
                        Location = new PlayerLocationDTO
                        {
                            PosX = playerLocation.PosX,
                            PosY = playerLocation.PosY,
                            Map = new MapDTO
                            {
                                Id = playerLocation.Map.Id,
                                Name = playerLocation.Map.Name,
                                MinLevel = playerLocation.Map.MinLevel,
                                MaxPlayers = playerLocation.Map.MaxPlayers
                            },
                            WorldModel = new WorldDTO
                            {
                                Id = playerLocation.World.Id,
                                Name = playerLocation.World.Name,
                                Description = playerLocation.World.Description
                            }
                        }
                    });
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[SUCCESS] Se obtuvieron {playerDTOs.Count} personajes para la cuenta {accountId}.");
                Console.ResetColor();

                return Ok(new GetPlayersResponseDTO { Players = playerDTOs });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Excepción al obtener los personajes: {ex.Message}");
                Console.ResetColor();
                return BadRequest($"Error al obtener los personajes: {ex.Message}");
            }
        }
    }

    public class CreatePlayerRequest
    {
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required string Race { get; set; }
        public required string Class { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using VentusServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using VentusServer.Services;
using Game.Models;
using Game.DTOs;

namespace VentusServer.Controllers
{
    [Route("player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;
        private readonly PlayerLocationService _playerLocationService;
        private readonly WorldService _worldService;
        private readonly MapService _mapService;
        private readonly FirebaseService _firebaseService;

        public PlayerController(PlayerService playerService, FirebaseService firebaseService,
        PlayerLocationService playerLocationService, WorldService worldService, MapService mapService)
        {
            _playerService = playerService;
            _playerLocationService = playerLocationService;

            _worldService = worldService;
            _mapService = mapService;

            _firebaseService = firebaseService;

        }

        // Crear un nuevo personaje
        [HttpPost("create-player")]
        [FirebaseAuthRequired] // Middleware de autenticación de Firebase
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest createPlayerRequest)
        {
            try
            {
                // Obtener el token de Bearer del encabezado Authorization
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token de autenticación no encontrado.");
                }

                // Verificar el token de Firebase
                var decodedToken = await _firebaseService.VerifyTokenAsync(token);
                var userId = decodedToken.Uid; // Obtener el userId del token

                // Crear el personaje
                Console.Write($"createPlayerRequest {createPlayerRequest}");
                // Guardar el personaje en la base de datos
                PlayerModel? newPlayer = await _playerService.CreatePlayer(userId, createPlayerRequest.Name, createPlayerRequest.Gender, createPlayerRequest.Race, createPlayerRequest.Class);
                if (newPlayer != null)
                {
                    return Ok(new { Message = "Personaje creado exitosamente", Player = newPlayer });

                }
                return BadRequest($"Error al crear el personaje");

            }
            catch (Exception ex)
            {
                // Manejar errores generales
                return BadRequest($"Error al crear el personaje: {ex.Message}");
            }
        }

        // Obtener todos los personajes del jugador autenticado
        [HttpGet("get-players")]
        [FirebaseAuthRequired] // Middleware de autenticación de Firebase
        public async Task<IActionResult> GetPlayers()
        {
            try
            {

                List<PlayerModel> players = await _playerService.GetAllPlayers();
                List<PlayerDTO> playerDTOs = new List<PlayerDTO>();
                Console.WriteLine($"Players {players}");
                Console.WriteLine($"Emtra 1");

                foreach (var player in players)
                {
                    PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(player.Id);
                Console.WriteLine($"Emtra 2 {playerLocation}");

                    if (playerLocation == null) continue;

                    World world = playerLocation.World;
                Console.WriteLine($"Emtra 3.1 {world}");

                    MapModel map = playerLocation.Map;
                Console.WriteLine($"Emtra 3 {map}");

                    var playerDTO = new PlayerDTO
                    {
                        Id = player.Id,
                        UserId = player.UserId,
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
                            World = new WorldDTO
                            {
                                Id = playerLocation.World.Id,
                                Name = playerLocation.World.Name,
                                Description = playerLocation.World.Description
                            }
                        }
                    };
                Console.WriteLine($"Emtra 5 {playerDTO.Id}");

                    // Crear el DTO de respuesta
                    playerDTOs.Add(playerDTO);
                 
                }
                   var responseDTO = new GetPlayersResponseDTO
                    {
                        Players = playerDTOs
                    };

                    return Ok(responseDTO);
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                return BadRequest($"Error al obtener los personajes: {ex.Message}");
            }
        }
    }

    // Clase de request para crear un personaje
    public class CreatePlayerRequest
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using FirebaseAdmin.Auth;
using System.Threading.Tasks;
using VentusServer.Models;
using System;
using Microsoft.Extensions.Configuration;

namespace VentusServer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountDAO _accountDAO;
        private readonly FirebaseService _firebaseService;

        public AuthController(IAccountDAO accountDAO, FirebaseService firebaseService)
        {
            _accountDAO = accountDAO;
            _firebaseService = firebaseService;
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
        {
            try
            {
                Console.WriteLine("Llega al inicio de la autenticación de Google.");

                FirebaseToken decodedToken = await _firebaseService.VerifyTokenAsync(request.IdToken);
                Console.WriteLine("Llega después de verificar el token.");

                // Obtén el userId real desde la propiedad Uid del token de Firebase
                string userId = decodedToken.Uid;  // Usar Uid en lugar de Claims["sub"]
                string? userEmail = decodedToken.Claims["email"]?.ToString();
                Console.WriteLine($"Llega: Email extraído: {userEmail}, UserId: {userId}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest("Email not found in the token.");
                }

                var existingUser = await _accountDAO.GetAccountByUserIdAsync(userId);
                Console.WriteLine($"Llega: Usuario {userId} encontrado en la base de datos.");
                var token = request.IdToken; //TokenUtils.GenerateToken(userId.ToString());

                if (existingUser != null)
                {
                    return Ok(new
                    {
                        login = true,
                        token
                    });
                }

                return Ok(new
                {
                    message = "User found in Firebase but not registered in the database. Please complete your registration.",
                    redirectToRegister = true,
                    token
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante la autenticación de Google: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                Console.WriteLine($"Iniciando registro para el usuario {request.Name} con correo {request.Email}.");

                // Decodificar el token para obtener el userId
                FirebaseToken decodedToken = await _firebaseService.VerifyTokenAsync(request.Token);

                string userId = decodedToken.Uid;  // Usar Uid en lugar de Claims["sub"]

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("El token no es válido.");
                }

                // Verificar si el usuario ya está registrado (por userId)
                var existingUserByUserId = await _accountDAO.GetAccountByUserIdAsync(userId);
                if (existingUserByUserId != null)
                {
                    return BadRequest("El usuario ya está registrado.");
                }

                // Verificar si ya existe un usuario con el mismo email
                var existingUserByEmail = await _accountDAO.GetAccountByEmailAsync(request.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest("El correo electrónico ya está en uso.");
                }

                // Verificar si ya existe un usuario con el mismo nombre
                var existingUserByName = await _accountDAO.GetAccountByNameAsync(request.Name);
                if (existingUserByName != null)
                {
                    return BadRequest("El nombre ya está en uso.");
                }

                // Verificar que se proporcione una contraseña
                if (string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Se requiere una contraseña.");
                }

                // Hashear la contraseña recibida
                var hashedPassword = HashPassword(request.Password);

                // Crear un nuevo objeto Account con los datos del usuario
                var newUser = new Account
                {
                    UserId = userId,
                    Email = request.Email,
                    Name = request.Name,
                    Password = hashedPassword,
                    CreatedAt = DateTime.UtcNow,
                    IsBanned = false,
                    LastIp = "test", // Aquí podrías obtener la IP real del cliente
                    Credits = 0
                };

                // Crear el usuario en la base de datos
                await _accountDAO.CreateAccountAsync(newUser);
                Console.WriteLine($"Registro exitoso para el usuario {request.Name}.");

                // Generar un token para el usuario registrado (si es necesario)
                var token = TokenUtils.GenerateToken(userId);

                return Ok(new
                {
                    Message = "Registro exitoso. Ahora puedes conectarte vía WebSocket.",
                    Token = token
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error durante el registro: {ex.Message}");
                return BadRequest($"Error: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

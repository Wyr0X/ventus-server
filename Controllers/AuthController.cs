using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using System.Threading.Tasks;
using VentusServer.Models;
using System;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly PostgresAccountDAO _accountDAO;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;

        public AuthController(
            PostgresAccountDAO accountDAO,
            JwtService jwtService,
            PasswordService passwordService)
        {
            _accountDAO = accountDAO;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine("[Login] Iniciando proceso de autenticación para: " + request.Email);
                var user = await _accountDAO.GetAccountByEmailAsync(request.Email);

                if (user == null)
                {
                    Console.WriteLine("[Login] Usuario no encontrado.");
                    return Unauthorized("Correo o contraseña incorrectos.");
                }

                if (!_passwordService.VerifyPassword(request.Password, user.Password))
                {
                    Console.WriteLine("[Login] Contraseña incorrecta.");
                    return Unauthorized("Correo o contraseña incorrectos.");
                }

                var token = _jwtService.GenerateToken(user.AccountId, user.Email);
                Console.WriteLine("[Login] Usuario autenticado correctamente.");
                return Ok(new { login = true, token });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Login] Error: " + ex.Message);
                return BadRequest("Error en Login: " + ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Console.WriteLine("[Register] Iniciando proceso de registro para: " + request.Email);

            var existingUser = await _accountDAO.GetAccountByEmailAsync(request.Email);
            if (existingUser != null)
            {
                Console.WriteLine("[Register] Error: Correo ya en uso.");
                return BadRequest("El correo ya está en uso.");
            }

            var accountId = Guid.NewGuid();

            var hashedPassword = _passwordService.HashPassword(request.Password);

            var newUser = new Account
            {
                AccountId = accountId,
                Email = request.Email,
                Name = request.Name,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                IsBanned = false,
                LastIp = "test",
                Credits = 0
            };

            await _accountDAO.CreateAccountAsync(newUser);
            Console.WriteLine("[Register] Usuario registrado con éxito: " + request.Email);

            var token = _jwtService.GenerateToken(accountId, request.Email);
            return Ok(new { message = "Registro exitoso. Ahora puedes conectarte.", token });
        }
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                Console.WriteLine("[ValidateToken] Iniciando validación de token...");

                // Intentar obtener el token desde las cookies
                var token = Request.Cookies["authToken"];

                // Si no hay token en las cookies, intentar obtenerlo desde el header de autorización
                if (string.IsNullOrEmpty(token) && Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                // Si sigue sin haber token, rechazar la petición
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("[ValidateToken] ❌ Token no encontrado.");
                    return Unauthorized(new { message = "Token no encontrado" });
                }

                // Validar el token
                var validatedAccountIdStr = _jwtService.ValidateToken(token);

                // Verificar si el accountId es un UUID válido
                if (!Guid.TryParse(validatedAccountIdStr, out Guid validatedAccountId))
                {
                    Console.WriteLine("[ValidateToken] ❌ Token inválido: no es un UUID válido.");
                    return Unauthorized(new { message = "Token inválido o expirado" });
                }

                // Buscar el usuario en la base de datos
                var user = await _accountDAO.GetAccountByAccountIdAsync(validatedAccountId);
                if (user == null)
                {
                    Console.WriteLine("[ValidateToken] ❌ Usuario no encontrado.");
                    return Unauthorized(new { message = "Usuario no encontrado" });
                }

                Console.WriteLine($"[ValidateToken] ✅ Token válido para {user.Email}");
                return Ok(new { accountId = user.AccountId, email = user.Email, name = user.Name });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ValidateToken] ⚠️ Error: {ex.Message}");
                return BadRequest(new { message = "Error en validación: " + ex.Message });
            }
        }

    }
}
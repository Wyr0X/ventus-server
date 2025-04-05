using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using System.Threading.Tasks;
using VentusServer.Models;
using System;
using VentusServer.DataAccess.Postgres;
using VentusServer.Services;

namespace VentusServer.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly JwtService _jwtService;
        private readonly PasswordService _passwordService;
        private readonly WebSocketServerController _webSocketServerController;

        public AuthController(
            AccountService accountService,
            JwtService jwtService,
            PasswordService passwordService,
            WebSocketServerController webSocketServerController)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _webSocketServerController = webSocketServerController;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                LoggerUtil.Log("Login", $"Iniciando proceso de autenticación para: {request.Email}", ConsoleColor.Cyan);
                var account = await _accountService.GetAccountByEmailAsync(request.Email);

                if (account == null)
                {
                    LoggerUtil.Log("Login", "Usuario no encontrado.", ConsoleColor.Yellow);
                    return Unauthorized("Correo o contraseña incorrectos.");
                }

                if (!_passwordService.VerifyPassword(request.Password, account.PasswordHash))
                {
                    LoggerUtil.Log("Login", "Contraseña incorrecta.", ConsoleColor.Yellow);
                    return Unauthorized("Correo o contraseña incorrectos.");
                }

                var sessionId = Guid.NewGuid();
                var token = _jwtService.GenerateToken(account.AccountId, account.Email, sessionId);


                var messageToClient = "";
                if (account.SessionId != Guid.Empty)
                {
                    LoggerUtil.Log("Login", $"El usuario ${account.AccountName} intento inciar desde otro navegador.", ConsoleColor.Green);

                    account.SessionId = sessionId;
                    messageToClient = "Había otra sesión activa. Se cerró automáticamente para continuar con este inicio de sesión.";
                    await _webSocketServerController.RemoveConnectionByAccountId(account.AccountId);
                    _accountService.Set(account.AccountId, account);
                }
                account.SessionId = sessionId;
                _accountService.Set(account.AccountId, account);
                LoggerUtil.Log("Login", "Usuario autenticado correctamente.", ConsoleColor.Green);
                return Ok(new { login = true, token, message = messageToClient });
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("Login", $"Error: {ex.Message}", ConsoleColor.Red);
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

            LoggerUtil.Log("Register", $"Iniciando proceso de registro para: {request.Email}", ConsoleColor.Cyan);

            var existingUser = await _accountService.GetAccountByEmailAsync(request.Email);
            if (existingUser != null)
            {
                LoggerUtil.Log("Register", "Error: Correo ya en uso.", ConsoleColor.Yellow);
                return BadRequest("El correo ya está en uso.");
            }

            var accountId = Guid.NewGuid();
            var hashedPassword = _passwordService.HashPassword(request.Password);

            var newAccount = new AccountModel
            {
                AccountId = accountId,
                Email = request.Email,
                AccountName = request.Name,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                IsBanned = false,
                LastIpAddress = "test",
                Credits = 0
            };

            await _accountService.CreateAccountAsync(newAccount);
            LoggerUtil.Log("Register", $"Usuario registrado con éxito: {request.Email}", ConsoleColor.Green);
            var sessionId = Guid.NewGuid();

            var token = _jwtService.GenerateToken(accountId, request.Email, sessionId);
            newAccount.SessionId = sessionId;
            return Ok(new { message = "Registro exitoso. Ahora puedes conectarte.", token });
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                LoggerUtil.Log("ValidateToken", "Iniciando validación de token...", ConsoleColor.Cyan);

                var token = Request.Cookies["authToken"];

                if (string.IsNullOrEmpty(token) && Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                if (string.IsNullOrEmpty(token))
                {
                    LoggerUtil.Log("ValidateToken", "❌ Token no encontrado.", ConsoleColor.Yellow);
                    return Unauthorized(new { message = "Token no encontrado" });
                }

                var (validateAccountId, sessionIdStr) = _jwtService.ValidateToken(token);
                if (validateAccountId == null)
                {
                    return Unauthorized(new { message = "Error al parsear accountId" });
                }
                if (!Guid.TryParse(validateAccountId.ToString(), out Guid validatedAccountId))
                {
                    LoggerUtil.Log("ValidateToken", "❌ Token inválido: no es un UUID válido.", ConsoleColor.Yellow);
                    return Unauthorized(new { message = "Token inválido o expirado" });
                }
                if (!Guid.TryParse(sessionIdStr, out Guid sessionId))
                {
                    return BadRequest(new { message = "Error al obtener el sessionId" });

                }
              
                var account = await _accountService.GetOrCreateAccountInCacheAsync(validatedAccountId);
                if (account != null && account.SessionId != sessionId)
                {
                    LoggerUtil.Log("Logout", "SessionId no corresponde a la sesión actual.", ConsoleColor.Yellow);
                    await _webSocketServerController.RemoveConnectionByAccountId(account.AccountId);
                    return Unauthorized(new { message = "SessionId no corresponde a la sesión actuala" });
                }
                if (account == null)
                {
                    LoggerUtil.Log("ValidateToken", "❌ Usuario no encontrado.", ConsoleColor.Yellow);
                    return Unauthorized(new { message = "Usuario no encontrado" });
                }

                LoggerUtil.Log("ValidateToken", $"✅ Token válido para {account.Email}", ConsoleColor.Green);
                return Ok(new { accountId = account.AccountId, email = account.Email, name = account.AccountName });
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("ValidateToken", $"⚠️ Error: {ex.Message}", ConsoleColor.Red);
                return BadRequest(new { message = "Error en validación: " + ex.Message });
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                LoggerUtil.Log("Logout", "Cerrando sesión...", ConsoleColor.Cyan);

                var token = Request.Cookies["authToken"];

                if (string.IsNullOrEmpty(token) && Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = Request.Headers["Authorization"].ToString();
                    if (authHeader.StartsWith("Bearer "))
                        token = authHeader.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Token no encontrado" });
                }

                var (accountIdStr, sessionIdStr) = _jwtService.ValidateToken(token);
                if (!Guid.TryParse(accountIdStr, out Guid accountId))
                {
                    return BadRequest(new { message = "Error al obtener el accountId" });

                }
                if (!Guid.TryParse(sessionIdStr, out Guid sessionId))
                {
                    return BadRequest(new { message = "Error al obtener el sessionId" });

                }
                AccountModel? account = await _accountService.GetOrLoadAsync(accountId);

                if (account == null)
                {
                    return Unauthorized(new { message = "Cuenta no encontrada" });
                }

                // Verificamos que el sessionId del token coincida con el actual
                if (account.SessionId != sessionId)
                {
                    LoggerUtil.Log("Logout", "Token no corresponde a la sesión actual.", ConsoleColor.Yellow);
                    return Unauthorized(new { message = "Token no corresponde a la sesión activa" });
                }

                account.SessionId = Guid.Empty;
                _accountService.Set(account.AccountId, account);
                await _webSocketServerController.RemoveConnectionByAccountId(account.AccountId);

                LoggerUtil.Log("Logout", $"Sesión cerrada para {account.Email}", ConsoleColor.Green);

                // Opcional: eliminar cookie
                Response.Cookies.Delete("authToken");

                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                LoggerUtil.Log("Logout", $"Error: {ex.Message}", ConsoleColor.Red);
                return BadRequest(new { message = "Error al cerrar sesión: " + ex.Message });
            }
        }

    }
}
using System.Net.WebSockets;
using Protos.Auth;
using Protos.Common;
using VentusServer.Models;
using VentusServer.Services;

public class AuthHandler
{
    private readonly JwtService _jwtService;
    private readonly AccountService _accountService;
    private readonly ResponseService _responseService;

    public AuthHandler(JwtService jwtService, AccountService accountService, ResponseService responseService)
    {
        _jwtService = jwtService;
        _accountService = accountService;
        _responseService = responseService;
    }

    public async Task<string?> HandleAuthMessage(AuthRequest authMessage, WebSocket webSocket)
    {
        Console.WriteLine($"📩 Token recibido: {authMessage.Token}");

        try
        {
            // Validar token y extraer datos
            var (validatedAccountIdStr, sessionIdStr) = _jwtService.ValidateToken(authMessage.Token);
            if (!Guid.TryParse(validatedAccountIdStr, out Guid accountId))
            {
                Console.WriteLine("⚠ Token inválido.");
                await _responseService.SendMessageAsync(webSocket, new AuthResponse { Success = false });
                return null;
            }

            // Obtener cuenta usando el servicio con caché
            var existingAccount = await _accountService.GetOrCreateAccountInCacheAsync(accountId);
            if (existingAccount != null)
            {
                // Actualizar último login y guardar cambios
                existingAccount.LastLogin = DateTime.UtcNow;
                await _accountService.SaveAccountAsync(existingAccount);

                Console.WriteLine($"✅ Último login actualizado para el usuario: {accountId}");

                await _responseService.SendMessageAsync(webSocket, new AuthResponse { Success = true });
                return accountId.ToString();
            }

            // Si no existe la cuenta
            Console.WriteLine("⚠ Usuario no encontrado.");
            var authResponse = new AuthResponse { Success = false };
            var serverMessage = new ServerMessage { AuthResponse = authResponse };
            await _responseService.SendMessageAsync(webSocket, serverMessage);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en autenticación: {ex.Message}");
            await _responseService.SendMessageAsync(webSocket, new AuthResponse { Success = false });
            return null;
        }
    }
}

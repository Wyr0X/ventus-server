using System.Net.WebSockets;
using Messages.Auth;
using ProtosCommon;
using VentusServer;
using VentusServer.DataAccess;
using VentusServer.DataAccess.Postgres;

public class AuthHandler
{
    private readonly JwtService _jwtService;
    private readonly PostgresAccountDAO _accountDAO;
    private readonly ResponseService _responseService;

    public AuthHandler(JwtService jwtService, PostgresAccountDAO accountDAO, ResponseService responseService)
    {
        _jwtService = jwtService;
        _accountDAO = accountDAO;
        _responseService = responseService;
    }

    public async Task<string?> HandleAuthMessage(AuthRequest authMessage, WebSocket webSocket)
    {
        Console.WriteLine($"📩 Token recibido: {authMessage.Token}");

        try
        {
            // Verificar el token JWT
            var validatedAccountIdStr = _jwtService.ValidateToken(authMessage.Token);
            if (!Guid.TryParse(validatedAccountIdStr, out Guid accountId))
            {
                Console.WriteLine("⚠ Token inválido.");
                await _responseService.SendMessageAsync(webSocket, new ServerMessage
                {
                    AuthResponse = new AuthResponse { Success = false }
                });
                return null;
            }

            // Intentar obtener la cuenta en la base de datos
            var existingAccount = await _accountDAO.GetAccountByAccountIdAsync(accountId);

            if (existingAccount != null)
            {
                // Si existe la cuenta, actualizar el último login
                existingAccount.LastLogin = DateTime.UtcNow;
                await _accountDAO.SaveAccountAsync(existingAccount);
                Console.WriteLine($"✅ Último login actualizado para el usuario: {accountId}");

                // Respuesta con autenticación exitosa
                await _responseService.SendMessageAsync(webSocket, new AuthResponse { Success = true });
                return accountId.ToString();
            }
            else
            {
                // Si no existe la cuenta, rechazar la autenticación
                Console.WriteLine("⚠ Usuario no encontrado.");
                await _responseService.SendMessageAsync(webSocket, new ServerMessage
                {
                    AuthResponse = new AuthResponse { Success = false }
                });
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en autenticación: {ex.Message}");
            await _responseService.SendMessageAsync(webSocket, new ServerMessage
            {
                AuthResponse = new AuthResponse { Success = false }
            });
            return null;
        }
    }
}

using System.Net.WebSockets;
using Messages.Auth;
using ProtosCommon;
using VentusServer;
using VentusServer.DataAccess;

public class AuthHandler
{
    private readonly FirebaseService _firebaseService;
    private readonly IAccountDAO _accountDAO;
    private readonly ResponseService _responseService;

    public AuthHandler(FirebaseService firebaseService, IAccountDAO accountDAO, ResponseService responseService)
    {
        _firebaseService = firebaseService;
        _accountDAO = accountDAO;
        _responseService = responseService;
    }

    public async Task<string?> HandleAuthMessage(AuthRequest authMessage, WebSocket webSocket)
    {
        Console.WriteLine($"📩 Token recibido: {authMessage.Token}");

        try
        {
            // Verificación del token de Firebase
            var userId = TokenUtils.DecodeTokenAndGetUserId(authMessage.Token);
            Console.WriteLine($"🔹 Usuario autenticado: {userId}");

            // Si no se pudo obtener el user_id, el proceso de autenticación falla
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("⚠ No se pudo obtener el user_id.");
                await _responseService.SendMessageAsync(webSocket, new ServerMessage
                {
                    AuthResponse = new AuthResponse { Success = false }
                });
                return null;
            }

            // Intentar obtener la cuenta en la base de datos
            var existingAccount = await _accountDAO.GetAccountByUserIdAsync(userId);

            if (existingAccount != null)
            {
                // Si existe la cuenta, actualizar el último login
                existingAccount.LastLogin = DateTime.UtcNow;

                await _accountDAO.SaveAccountAsync(existingAccount);
                Console.WriteLine($"✅ Último login actualizado para el usuario: {userId}");

                // Respuesta con autenticación exitosa y cuenta encontrada
                await _responseService.SendMessageAsync(webSocket, new AuthResponse { Success = true });
                return userId;
            }
            else
            {
                // Si no existe la cuenta, informar al usuario para que complete el registro
                Console.WriteLine($"📝 Cuenta no encontrada. Redirigiendo al registro... {userId}");

                var authResponse = new AuthResponse { Success = true };

                await _responseService.SendMessageAsync(webSocket, authResponse);
                return null; // Indicar que el usuario debe completar el registro
            }
        }
        catch (Exception ex)
        {
            // Si ocurrió un error al intentar autenticar con Firebase
            Console.WriteLine($"❌ Error en autenticación: {ex.Message}");
            await _responseService.SendMessageAsync(webSocket, new ServerMessage
            {
                AuthResponse = new AuthResponse { Success = false }
            });
            return null;
        }
    }
}

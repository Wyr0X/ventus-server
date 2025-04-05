using System;

public class WebSocketAuthenticationService
{
    private readonly JwtService _jwtService;

    public WebSocketAuthenticationService()
    {
        _jwtService = new JwtService();
    }

    public bool TryAuthenticate(string token, out Guid accountId)
    {
        accountId = Guid.Empty;
        try
        {
            var accountIdStr = _jwtService.ValidateToken(token);
            var result = Guid.TryParse(accountIdStr, out accountId);

            LoggerUtil.Log("AuthService", result
                ? $"Token válido. AccountId: {accountId}"
                : $"Token inválido: no se pudo convertir a Guid", 
                result ? ConsoleColor.Green : ConsoleColor.Red);

            return result;
        }
        catch (Exception ex)
        {
            LoggerUtil.Log("AuthService", $"Error validando token: {ex.Message}", ConsoleColor.Red);
            return false;
        }
    }
}

using System;
using VentusServer.Services;

public class WebSocketAuthenticationService
{
    private readonly JwtService _jwtService;
    private readonly AccountService _accountService;


    public WebSocketAuthenticationService(AccountService accountService)
    {
        _jwtService = new JwtService();
        _accountService = accountService;
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

    public bool verifyToken( Guid accountId, string currenToken){
        AccountModel? account = _accountService.GetIfLoaded(accountId);
        if (account != null){
            if (account.ValidToken == currenToken){
                return true;
            }
        }
        return false;
    }
}

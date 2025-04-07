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
            var (accountIdStr, sessionIdStr) = _jwtService.ValidateToken(token);
            var result = Guid.TryParse(accountIdStr, out accountId);

            LoggerUtil.Log(LoggerUtil.LogTag.AuthService, result
                ? $"Token válido. AccountId: {accountId}"
                : $"Token inválido: no se pudo convertir a Guid");

            return result;
        }
        catch (Exception ex)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.AuthService, $"Error validando token: {ex.Message}");
            return false;
        }
    }

}

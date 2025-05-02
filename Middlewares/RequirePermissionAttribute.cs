using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VentusServer.Domain.Enums;
using VentusServer.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly Permission _requiredPermission;

    public RequirePermissionAttribute(Permission requiredPermission = Permission.AdminPanel)
    {
        _requiredPermission = requiredPermission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, "Iniciando verificación de permisos...", "Start");

        var permissionService = context.HttpContext.RequestServices.GetService<PermissionService>();
        var IAccountService = context.HttpContext.RequestServices.GetService<IAccountService>();

        if (permissionService == null || IAccountService == null)
        {
            var missingServices = new List<string>();
            if (permissionService == null) missingServices.Add(nameof(PermissionService));
            if (IAccountService == null) missingServices.Add(nameof(IAccountService));

            LoggerUtil.Log(
                LoggerUtil.LogTag.RequirePermissionAttribute,
                $"Fallo al obtener servicios desde el contenedor DI. Faltan: {string.Join(", ", missingServices)}. Permiso requerido: {_requiredPermission}",
                "Error",
                isError: true
            );

            context.Result = new StatusCodeResult(500);
            return;
        }

        string? accountIdStr = null;

        // 1. Intentamos obtener desde HttpContext.Items
        if (context.HttpContext.Items.TryGetValue(HttpContextKeys.AccountId, out var accountIdObj) &&
            accountIdObj is string idStr)
        {
            accountIdStr = idStr;
        }

        // 2. Si no está, intentamos desde Authorization header
        if (string.IsNullOrEmpty(accountIdStr))
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Replace("Bearer ", "").Trim();
                LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Token extraído: {token}", "Token");

                var (accountIdFromToken, sessionId) = JwtService.ValidateToken(token);
                accountIdStr = accountIdFromToken;

                if (!string.IsNullOrEmpty(accountIdStr))
                {
                    context.HttpContext.Items[HttpContextKeys.AccountId] = accountIdStr;
                    context.HttpContext.Items[HttpContextKeys.SessionId] = sessionId;
                    LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"AccountId obtenido del token: {accountIdStr}", "Token");
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, "Token inválido o expirado.", "Token", isError: true);
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, "No se encontró token válido en el header Authorization.", "Token", isError: true);
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        // 3. Validamos que el accountId sea un GUID válido
        if (!Guid.TryParse(accountIdStr, out var parsedAccountId))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"El AccountId '{accountIdStr}' no es un GUID válido.", "Check", isError: true);
            context.Result = new UnauthorizedResult();
            return;
        }

        var accountId = parsedAccountId;

        LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Verificando cuenta con ID {accountId}.", "Check");

        var account = await IAccountService.GetOrCreateAccountInCacheAsync(accountId);
        if (account == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Cuenta no encontrada en caché para ID {accountId}.", "Check", isError: true);
            context.Result = new UnauthorizedResult();
            return;
        }

        LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Chequeando permiso '{_requiredPermission}' para la cuenta '{accountId}'.", "Permission");

        var hasPermission = await permissionService.HasPermission(account, _requiredPermission);
        if (!hasPermission)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Cuenta '{accountId}' no tiene el permiso requerido: {_requiredPermission}.", "Permission", isError: true);
            context.Result = new ForbidResult();
            return;
        }

        LoggerUtil.Log(LoggerUtil.LogTag.RequirePermissionAttribute, $"Permiso '{_requiredPermission}' concedido para la cuenta '{accountId}'.", "Permission");
    }
}
public static class HttpContextKeys
{
    public const string AccountId = "AccountId";
    public const string SessionId = "SessionId";
}

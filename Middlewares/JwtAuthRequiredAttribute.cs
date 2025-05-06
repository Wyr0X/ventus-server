using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace VentusServer.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthRequiredAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, "Middleware JwtAuthRequired iniciado.");



            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();



            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, "ERROR: No se encontró el token en el header.", isError: true);

                context.Result = new UnauthorizedObjectResult("Autenticación requerida.");
                return;
            }

            var token = authorizationHeader.Replace("Bearer ", "").Trim();
            LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, $"Token extraído: {token}");


            var (accountId, sessionId) = JwtService.ValidateToken(token); // Verificamos el token
            LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, $"UserId obtenido del token: {accountId}");


            if (string.IsNullOrEmpty(accountId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, "ERROR: Token inválido o expirado.", isError: true);

                context.Result = new UnauthorizedObjectResult("Token inválido o expirado.");
                return;
            }

            context.HttpContext.Items["AccountId"] = accountId;
            context.HttpContext.Items["SessionId"] = sessionId;

            LoggerUtil.Log(LoggerUtil.LogTag.JwtAuthRequired, $"AccountId almacenado en HttpContext: {accountId}");


            await next();
        }
    }
}

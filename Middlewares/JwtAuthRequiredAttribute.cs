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
            Console.WriteLine("[JwtAuthRequired] Middleware iniciado.");


            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();



            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                Console.WriteLine("[JwtAuthRequired] ERROR: No se encontró el token en el header.");
                context.Result = new UnauthorizedObjectResult("Autenticación requerida.");
                return;
            }

            var token = authorizationHeader.Replace("Bearer ", "").Trim();
            Console.WriteLine($"[JwtAuthRequired] Token extraído: {token}");

            var (accountId, sessionId) = JwtService.ValidateToken(token); // Verificamos el token
            Console.WriteLine($"[JwtAuthRequired] UserId obtenido del token: {accountId}");

            if (string.IsNullOrEmpty(accountId))
            {
                Console.WriteLine("[JwtAuthRequired] ERROR: Token inválido o expirado.");
                context.Result = new UnauthorizedObjectResult("Token inválido o expirado.");
                return;
            }

            context.HttpContext.Items["AccountId"] = accountId;
            context.HttpContext.Items["SessionId"] = sessionId;

            Console.WriteLine($"[JwtAuthRequired] AccountId almacenado en HttpContext: {accountId}");

            await next();
        }
    }
}

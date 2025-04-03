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

            var jwtService = context.HttpContext.RequestServices.GetService<JwtService>();
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            Console.WriteLine($"[JwtAuthRequired] Authorization Header: {authorizationHeader}");

            if (jwtService == null)
            {
                Console.WriteLine("[JwtAuthRequired] ERROR: JwtService no está disponible.");
                context.Result = new UnauthorizedObjectResult("Autenticación requerida.");
                return;
            }

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                Console.WriteLine("[JwtAuthRequired] ERROR: No se encontró el token en el header.");
                context.Result = new UnauthorizedObjectResult("Autenticación requerida.");
                return;
            }

            var token = authorizationHeader.Replace("Bearer ", "").Trim();
            Console.WriteLine($"[JwtAuthRequired] Token extraído: {token}");

            var accountId = jwtService.ValidateToken(token); // Verificamos el token
            Console.WriteLine($"[JwtAuthRequired] UserId obtenido del token: {accountId}");

            if (string.IsNullOrEmpty(accountId))
            {
                Console.WriteLine("[JwtAuthRequired] ERROR: Token inválido o expirado.");
                context.Result = new UnauthorizedObjectResult("Token inválido o expirado.");
                return;
            }

            context.HttpContext.Items["AccountId"] = accountId; // Guardamos el UserId en el contexto
            Console.WriteLine($"[JwtAuthRequired] AccountId almacenado en HttpContext: {accountId}");

            await next();
        }
    }
}

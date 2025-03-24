using Microsoft.AspNetCore.Http;
using VentusServer;

public class FirebaseAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, FirebaseService firebaseService)
    {
        // Si la ruta no está marcada como protegida, omite la verificación de token
        var endpoint = context.GetEndpoint();
        if (endpoint == null || endpoint.Metadata?.Any(m => m is FirebaseAuthRequiredAttribute) != true)
        {
            await _next(context); // Rutas no protegidas
            return;
        }

        // Verificar el token de Firebase
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Token not found.");
            return;
        }

        try
        {
            // Verificar el token con Firebase
            var decodedToken = await firebaseService.VerifyTokenAsync(token);
            // Adjuntar el userId al contexto para que sea accesible en los controladores
            context.Items["UserId"] = decodedToken.Uid;
            await _next(context); // Continuar con la solicitud
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid token.");
        }
    }
}

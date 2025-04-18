using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

public static class TokenUtils
{
    private static string _secretKey = "TuClaveSuperSecretaCon32CaracteresOmas!";


    // Generar el token JWT que incluye el userId
    public static string GenerateToken(string userId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId) // Incluir el userId en el token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "ventus", // El emisor del token
            audience: "ventus", // La audiencia esperada del token
            claims: claims,
            expires: DateTime.Now.AddHours(24), // El token expirará en 1 hora
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Decodificar el token y obtener el userId
    public static string? DecodeTokenAndGetUserId(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Si el token no está expirado, puedes validarlo
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

            // El userId está en el claim "nameid"
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
        catch (Exception ex)
        {
            // Si hay un error al decodificar el token, retornamos null
            Console.WriteLine($"Error al decodificar el token: {ex.Message}");
            return null;
        }
    }
}

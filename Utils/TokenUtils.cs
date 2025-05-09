using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

public static class TokenUtils
{
    private static string _secretKey = "TuClaveSuperSecretaCon32CaracteresOmas!";
    private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();
    private const string Issuer = "ventus";
    private const string Audience = "ventus";
    // Generar el token JWT que incluye el userId
    public static string GenerateToken(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("El userId no puede ser nulo o vacío.", nameof(userId));
        var now = TimeProvider.UtcNow(); // Almacenar el valor de TimeProvider.now() en una variable local

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId) // Incluir el userId en el token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = TokenHandler.WriteToken(new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: now.AddHours(24), // Usar la variable local
            signingCredentials: creds
        ));

        return token;
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
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"Error de seguridad al decodificar el token: {ex.Message}");
            return null;
        }
    }
}

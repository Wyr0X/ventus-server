using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public static class JwtService
{
    private static readonly string _jwtSecret = "6YpV@cQ3z!mJtX7#dFk$wN9rLp*2HsG0"; // Se puede cambiar a un valor desde configuración si es necesario
    private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler(); // Reutilizar el TokenHandler

    public static string GenerateToken(Guid accountId, string email, Guid sessionId)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("accountId", accountId.ToString()),
                new Claim("email", email),
                new Claim("sessionId", sessionId.ToString())
            }),
            Expires = TimeProvider.UtcNow().AddHours(2), // Usar TimeProvider para centralizar el acceso al tiempo
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = TokenHandler.CreateToken(tokenDescriptor);
        return TokenHandler.WriteToken(token);
    }

    public static (string? accountId, string? sessionId) ValidateToken(string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = TokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            // Validar la expiración del token
            if (jwtToken.ValidTo < TimeProvider.UtcNow())
                return (null, null);

            var accountIdStr = jwtToken.Claims.First(x => x.Type == "accountId").Value;
            var sessionId = jwtToken.Claims.First(x => x.Type == "sessionId").Value;
            return (accountIdStr, sessionId);
        }
        catch (SecurityTokenException) // Capturar excepciones específicas
        {
            return (null, null);
        }
    }
}

// Servicio de tiempo centralizado

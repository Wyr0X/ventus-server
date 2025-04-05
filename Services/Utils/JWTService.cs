using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

public class JwtService
{
    private readonly string _jwtSecret;

    public JwtService()
    {
        //_jwtSecret = configuration["Jwt:Secret"];
        _jwtSecret = "6YpV@cQ3z!mJtX7#dFk$wN9rLp*2HsG0";
    }

    public string GenerateToken(Guid accountId, string email, Guid sessionId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
            new Claim("accountId", accountId.ToString()),
            new Claim("email", email),
            new Claim("sessionId", sessionId.ToString())
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (string? accountId, string? sessionId) ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo < DateTime.UtcNow)
                return (null, null);

            var accountIdStr = jwtToken.Claims.First(x => x.Type == "accountId").Value;
            var sessionId = jwtToken.Claims.First(x => x.Type == "sessionId").Value;
            return (accountIdStr, sessionId);
        }
        catch
        {
            return (null, null);
        }
    }
}
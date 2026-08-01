using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApiCoreSeed.IntegrationTests.Infrastructure;

public static class AuthenticationHelper
{
    public const string TestSecret = "X-BURGUER@COCA-2-INTEGRATION-TEST-SECRET-2026-WITH-HS384-SIZE";
    public const string Issuer = "Restaurante";
    public const string Audience = "https://localhost";

    public static string CreateToken(params (string Type, string Value)[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(TestSecret));
        var tokenClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, $"teste-{Guid.NewGuid():N}@restaurante.local")
        };

        tokenClaims.AddRange(claims.Select(claim => new Claim(claim.Type, claim.Value)));

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: tokenClaims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

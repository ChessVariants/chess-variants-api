using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChessVariantsAPI.Authentication;

/// <summary>
/// Utility for generating a JSON web token.
/// </summary>
public class JWTUtils
{
    private readonly string _secretKey;

    public JWTUtils(string jwtSecretKey)
    {
        _secretKey = jwtSecretKey;
    }

    public string GenerateToken(string username, string? email, int expirationDays=180)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(CreateClaims(username, email)),
            Expires = DateTime.UtcNow.AddDays(expirationDays),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static Claim[] CreateClaims(string username, string? email)
    {
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, username)};
        if (email != null) claims.Add(new Claim(ClaimTypes.Email, email));
        return claims.ToArray();
    }
}
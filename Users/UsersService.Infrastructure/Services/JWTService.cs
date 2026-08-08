using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UsersService.Application.Services;
using UsersService.Domain.Models.Auth;
using UsersService.Infrastructure.Models;

namespace UsersService.Infrastructure.Services;

public class JWTService : IJWTService
{
    private readonly JWTSettings _jwtSettings;

    public JWTService(IOptions<JWTSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public string CreateJWTToken(Guid userId, string login, Role role)
    {
        var claims = new Dictionary<string, object>
        {
            [ClaimTypes.NameIdentifier] = userId.ToString(),
            [ClaimTypes.Role] = role.ToString(),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Claims = claims,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeMinutes),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = creds
        };

        var result = new JsonWebTokenHandler().CreateToken(descriptor);

        return result;
    }
}

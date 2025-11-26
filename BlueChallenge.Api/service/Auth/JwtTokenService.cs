using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlueChallenge.Api.Configuration;
using BlueChallenge.Api.Model.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlueChallenge.Api.Service.Auth;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthenticationResult GenerateToken(UserModel user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var signingKey = Encoding.UTF8.GetBytes(_options.SigningKey);
        if (signingKey.Length == 0)
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpiresInMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Credentials.Email.Address),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(token);

        return new AuthenticationResult(accessToken, expiresAt);
    }
}

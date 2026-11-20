using LetsTalk.DTOs.Auth;
using LetsTalk.Options;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LetsTalk.Domain.Entities;

namespace LetsTalk.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _options;
        
        public TokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public TokenResponse GenerateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpiryMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt.UtcDateTime,
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            var accessToken = handler.CreateToken(descriptor);

            return new TokenResponse(accessToken, expiresAt);
        }
    }
}

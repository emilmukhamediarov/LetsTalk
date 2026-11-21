using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LetsTalk.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}

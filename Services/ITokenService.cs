using LetsTalk.Domain.Entities;
using LetsTalk.DTOs.Auth;

namespace LetsTalk.Services
{
    public interface ITokenService
    {
        TokenResponse GenerateToken(AppUser user);
    }
}

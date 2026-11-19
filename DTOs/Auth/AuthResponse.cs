using LetsTalk.DTOs.AppUsers;

namespace LetsTalk.DTOs.Auth
{
    public record AuthResponse(
        string AccessToken,
        DateTimeOffset ExpiresAt,
        AppUserResponse User
        );
}

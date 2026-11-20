using LetsTalk.DTOs.AppUsers;

namespace LetsTalk.DTOs.Auth
{
    public record AuthResponse(
        TokenResponse Token,
        AppUserResponse User
        );
}

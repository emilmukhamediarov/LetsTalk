namespace LetsTalk.DTOs.Auth
{
    public record TokenResponse(
        string AccessToken,
        DateTimeOffset ExpiresAt
        );
}

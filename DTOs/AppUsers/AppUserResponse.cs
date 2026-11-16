namespace LetsTalk.DTOs.AppUsers
{
    public record AppUserResponse(
        Guid Id,
        string UserName,
        DateTimeOffset CreatedAt,
        DateTimeOffset? LastSeenAt);
}

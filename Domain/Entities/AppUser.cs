using Microsoft.AspNetCore.Identity;

namespace LetsTalk.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public required string DisplayName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? LastSeenAt { get; set; }
    }
}

namespace LetsTalk.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastSeenAt { get; set; }
        public ICollection<ConversationParticipant> Conversations { get; set; } = [];
        public ICollection<Message> SentMessages { get; set; } = [];
    }
}

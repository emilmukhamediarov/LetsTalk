namespace LetsTalk.Domain.Entities
{
    public class ConversationParticipant
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = null;
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;

        // Tracks read receipts & unread counts efficiently per user
        public Guid? LastReadMessageId { get; set; }
        public DateTimeOffset? LastReadAt { get; set; }
    }
}

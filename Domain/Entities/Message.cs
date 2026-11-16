using LetsTalk.Domain.Enums;

namespace LetsTalk.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public Guid SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;
        public required string Content { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Sent;
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? EditedAt { get; set; }
    }
}



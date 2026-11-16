namespace LetsTalk.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        
        //Denormalized fields for fast sidebar rendering
        public DateTimeOffset? LastMessageAt { get; set; }
        public string? LastMessagePreview { get; set; }

        //Canonical key for 1-on-1 uniqueness (e.g., "{smallerGuid}_{largerGuid}")
        public required string DirectParticipantKey { get; set; }

        public ICollection<ConversationParticipant> Participants { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];
    }
}

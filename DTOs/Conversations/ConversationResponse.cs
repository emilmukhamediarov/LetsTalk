using LetsTalk.DTOs.ConversationParticipants;

namespace LetsTalk.DTOs.Conversations
{
    public record ConversationResponse(
        Guid Id,
        DateTimeOffset CreatedAt,
        DateTimeOffset? LastMessageAt,
        string? LastMessagePreview,
        IReadOnlyList<ConversationParticipantResponse> Participants);
}

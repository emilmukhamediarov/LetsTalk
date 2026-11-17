using LetsTalk.Domain.Enums;

namespace LetsTalk.DTOs.Messages
{
    public record MessageResponse(
        Guid Id,
        Guid ConversationId,
        Guid SenderId,
        string SenderUserName,
        string Content,
        MessageStatus Status,
        DateTimeOffset SentAt,
        DateTimeOffset? EditedAt
        );
}

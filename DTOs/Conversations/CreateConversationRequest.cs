using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.Conversations
{
    public class CreateConversationRequest
    {
        [Required]
        public Guid? SenderId { get; set; }
        [Required]
        public Guid? ConnectToId { get; set; }
    }
}

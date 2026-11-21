using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.Conversations
{
    public class CreateConversationRequest
    {
        [Required]
        public Guid? ConnectToId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.Messages
{
    public class SendMessageRequest
    {
        [Required]
        public Guid? SenderId { get; set; }
        [Required]
        [MaxLength(1000)]
        public string? Content { get; set; }
    }
}

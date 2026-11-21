using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.Messages
{
    public class SendMessageRequest
    {
        [Required]
        [MaxLength(1000)]
        public string? Content { get; set; }
    }
}

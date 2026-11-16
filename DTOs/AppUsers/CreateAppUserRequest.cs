using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.AppUsers
{
    public class CreateAppUserRequest
    {
        [Required]
        [MaxLength(20)]
        public required string UserName { get; set; } = null!;
    }
}

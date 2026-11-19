using System.ComponentModel.DataAnnotations;

namespace LetsTalk.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength (20)]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength (256)]
        public string? Email { get; set; }

        [Required]
        [MinLength (8)]
        public string? Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LetsTalk.Options
{
    public class JwtOptions
    {
        [Required]
        public required string Issuer { get; set; }
        [Required]
        public required string Audience { get; set; }
        [Range(1, 1440)]
        public required int ExpiryMinutes { get; set; }
        [Required]
        [MinLength (32)]
        public required string SecretKey { get; set; }
    }
}

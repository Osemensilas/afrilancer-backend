using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class VerifyUserDto
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Token { get; set; } = "";
    }
}
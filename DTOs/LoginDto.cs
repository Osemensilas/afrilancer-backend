using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "All fields required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";
    }
}
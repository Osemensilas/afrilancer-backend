using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class ResendRegEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
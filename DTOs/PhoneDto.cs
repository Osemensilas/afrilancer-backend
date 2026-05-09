using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class PhoneDto
    {
        [Required(ErrorMessage = "All field reqiored")]
        public string Phone { get; set; } = "";
    }
}
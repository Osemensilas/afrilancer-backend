using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class PhoneCodeDto
    {
        [Required(ErrorMessage = "All field required")]
        public string Code { get; set; } = "";
    }
}
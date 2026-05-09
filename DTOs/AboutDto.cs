using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class AboutDto
    {
        public string UserId { get; set;} = "";
        
        [Required(ErrorMessage = "All field required")]
        public string About { get; set; } = "";
    }
}
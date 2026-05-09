using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class BasicDto
    {
        [Required(ErrorMessage = "All field required")]
        public string Location { get; set; } = "";

        [Required(ErrorMessage = "All field required")]
        public string Title { get; set; } = "";
    }
}
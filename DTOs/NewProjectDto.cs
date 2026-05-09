using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class NewProjectDto
    {
        [Required(ErrorMessage = "All field required")]
        public string Title { get; set; } = "";

        [Required(ErrorMessage = "All field required")]
        public DateTime StartedAt { get; set; }

        [Required(ErrorMessage = "All field required")]
        public DateTime EndedAt { get; set; }

        [Required(ErrorMessage = "All field required")]
        public string Role { get; set; } = "";

        [Required(ErrorMessage = "All field required")]
        public string Description { get; set; } = "";

        public List<IFormFile>? Images { get; set; }
    }
}
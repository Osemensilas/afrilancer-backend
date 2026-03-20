using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class NewJobDto
    {
        [Required(ErrorMessage = "All field required")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "All field required")]
        public List<string> Skill { get; set; } = new();

        [Required(ErrorMessage = "All field required")]
        public int Budget { get; set; } = 0;

        [Required(ErrorMessage = "All field required")]
        public string Duration { get; set; } = "";

        [Required(ErrorMessage = "All field required")]
        public bool Paid = false;
    }
}
using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class NewReviewDto
    {
        [Required(ErrorMessage = "All field required")]
        public string ReviewId { get; set;} = "";

        [Required(ErrorMessage = "All field required")]
        public string Comment { get; set;} = "";

        [Required(ErrorMessage = "All field required")]
        public int Rating { get; set;} = 0;

        [Required(ErrorMessage = "All field required")]
        public string JobDescription { get; set; } = "";
    }
}
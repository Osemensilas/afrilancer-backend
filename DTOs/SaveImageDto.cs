using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class SaveImageDto
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile? Image { get; set;}
    }
}
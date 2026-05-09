using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class DeleteProjectDto
    {
        [Required(ErrorMessage = "Project Id Required")]
        public string ProjectId { get; set;} = "";
    }
}
using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class SkillDto
    {
        public string UserId { get; set;} = "";
        public List<string> Skill { get; set; } = new(); 
    }
}
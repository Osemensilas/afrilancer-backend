namespace Afrilancer.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string ProjectId { get; set; } = "";
        public string Title { get; set; } = "";
        public DateTime? StartedAt { get; set;}
        public DateTime? EndedAt { get; set;}
        public string Role { get; set; } = "";
        public string Description { get; set;} = "";
        public string Skills { get; set; } = "[]";
        public string Images { get; set; } = "[]";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
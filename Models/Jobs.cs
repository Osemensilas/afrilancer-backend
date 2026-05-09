using System.Text.Json;

namespace Afrilancer.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string JobId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set;} = "";
        public string Duration { get; set; } = "";
        public int Budget { get; set; } = 0;
        public int Coin { get; set; } = 0;
        public string Skill { get; set; } = "[]";
        public bool Paid { get; set; } = false;
        public string Status { get; set; } = "open";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
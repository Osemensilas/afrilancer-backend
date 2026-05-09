namespace Afrilancer.Models
{
    public class Applied
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string JobId { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
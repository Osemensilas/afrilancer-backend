namespace Afrilancer.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string ReviewedId { get; set;} = "";
        public string Comment { get; set;} = "";
        public int Rating { get; set; } = 0;
        public string JobDescription { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
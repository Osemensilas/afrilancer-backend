namespace Afrilancer.Models
{
    public class Coin
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
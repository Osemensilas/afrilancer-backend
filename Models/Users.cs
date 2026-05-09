namespace Afrilancer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Country { get; set; } = "";
        public bool MailTips { get; set; } = false;
        public bool EmailVerified { get; set; } = false;
        public string Token { get; set; } = "";
        public string AccountType { get; set; } = "";
        public string Title { get; set; } = "";
        public string Location { get; set; } = "";
        public string Image { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
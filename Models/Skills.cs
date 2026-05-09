namespace Afrilancer.Models
{
    public class Skill
    {
        public int Id { get; set;}
        public string UserId { get; set;} = "";
        public string Skills { get; set; } = "[]";
        public string About { get; set; } = "";
        public string Phone { get; set; } = "";
        public string PhoneOtp { get; set; } = "";
        public DateTime? PhoneOtpExpiry { get; set; }
        public bool PhoneVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
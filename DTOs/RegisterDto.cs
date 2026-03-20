using System.ComponentModel.DataAnnotations;

namespace Afrilancer.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "All Fields are required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Invalid First Name Lenght")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "All Fields are required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Invalid Last Name Lenght")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Password1 { get; set; } = "";

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password1", ErrorMessage = "Passwords do not match")]
        public string Password2 { get; set; } = "";

        [Required(ErrorMessage = "Email tips required")]
        public bool MailTips { get; set; } = false;

        [Required]
        public bool EmailVerified = false;

        [Required]
        public string Token = "";

        [Required]
        public string AccountType { get; set;} = "";
    }
}
using Afrilancer.DTOs;
using Afrilancer.Models;
using Afrilancer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using MailKit.Net.Smtp;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Afrilancer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == dto.Email.ToLower() && u.EmailVerified);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already registered" });
            }

            var emailExistWithFalse = _context.Users.FirstOrDefault(u => u.Email == dto.Email.ToLower() && u.EmailVerified == false);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password1);

            string userId = $"AFR-{Guid.NewGuid().ToString("N").Substring(0,8).ToUpper()}";

            string token;
            
            if (emailExistWithFalse == null)
            {   
                token = $"XYZ-{Guid.NewGuid().ToString("N").Substring(0,8).ToUpper()}";

                var user = new User
                {
                    UserId = userId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email.ToLower(),
                    Country = dto.Country,
                    Password = hashedPassword,
                    MailTips = dto.MailTips,
                    EmailVerified = false,
                    Token = token,
                    AccountType = dto.AccountType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                token = $"XYZ-{Guid.NewGuid().ToString("N").Substring(0,8).ToUpper()}";
                emailExistWithFalse.Token = token;
                emailExistWithFalse.Password = hashedPassword;
                emailExistWithFalse.FirstName = dto.FirstName;
                emailExistWithFalse.LastName = dto.LastName;
                emailExistWithFalse.Country = dto.Country;
                emailExistWithFalse.AccountType = dto.AccountType;
                emailExistWithFalse.CreatedAt = DateTime.UtcNow;
                emailExistWithFalse.MailTips = dto.MailTips;
                emailExistWithFalse.EmailVerified = false;
                emailExistWithFalse.AccountType = dto.AccountType;

                await _context.SaveChangesAsync();
            }

            var newEmail = dto.Email;

            try
            {
                var result = await SendMessage(newEmail, token);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Email sending failed", error = ex.Message });
            }
            
            return Ok( new
            {
                message = "Account Created",
                status = "success",
            });
        }

        [HttpPost("resend-confim-mail")]
        public async Task<IActionResult> ResendRegEmail([FromBody] ResendRegEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = ModelState });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email.ToLower() && u.EmailVerified == false);

            if (user == null){
                return BadRequest(new { status = "error", message = "User could not be found" });
            }

            var newEmail = dto.Email;
            string token = $"XYZ-{Guid.NewGuid().ToString("N").Substring(0,8).ToUpper()}";

            user.Token = token;

            await _context.SaveChangesAsync();

            try
            {
                var result = await SendMessage(newEmail, token);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Email sending failed", error = ex.Message });
            }

            return Ok (new {
                status = "success", 
                email = dto.Email,
                message = "Message Sent"
            });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = "error", message = ModelState });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email.ToLower());

            if (user == null){
                return BadRequest(new { status = "error", message = "User not registered" });
            }

            user.EmailVerified = true;
            await _context.SaveChangesAsync();

            return Ok( new
            {
                message = "User verified",
                status = "success",
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
            {
                return BadRequest(new {status = "error", message = "User do not exist"});
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!isValidPassword)
            {
                return BadRequest(new {status = "error", message = "Invalid password"});
            }

            return Ok(new
            {
                status = "success",
                message = "Login successful",
                token = GenerateJwt(user),
                user_type = user.AccountType,
                user = new
                {
                    userId = user.UserId,
                    email = user.Email,
                    firtname = user.FirstName,
                    lastname = user.LastName,
                    user_type = user.AccountType,
                }
            });
        }

        [HttpPost("logout")]
        public async Task<object> Logout()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(new
            {
                status = "success",
                message = "logout"
            });
        }

        private async Task<object> SendMessage(string newEmail, string token)
        {

            try
            {
                var verifyLink = $"http://localhost:3000/register/verify-email?email={newEmail}&token={token}";

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("contact@iruhost.com"));
                email.To.Add(MailboxAddress.Parse(newEmail));
                email.Subject = "Verify Your Email Address";
                email.Body = new TextPart("html")
                {
                    Text = $"<p>Click below to verify your account:</p><a href='{verifyLink}'>Verify Account</a>"
                };

                using var smtp = new SmtpClient();

                await smtp.ConnectAsync("mail.iruhost.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync("noreply@iruhost.com", "Onion$101Banks");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return new
                {
                    status = "successful",
                    message = "Message Sent"
                };
            }
            catch (Exception ex)
            {

                return new
                {
                    status = "failed",
                    message = ex.Message 
                };
            }
        }

        private string GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
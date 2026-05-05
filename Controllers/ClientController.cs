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
using System.Text.Json;

namespace Afrilancer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ClientController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpPost("create-new-job")]
        public async Task<IActionResult> NewJob([FromBody] NewJobDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return Unauthorized();
            }

             var projectId = $"PROJ-{Guid.NewGuid().ToString("N").Substring(0,16).ToUpper()}";

            var project = new Job
            {
                UserId = userId,
                JobId = projectId,
                Description = dto.Description,
                Duration = dto.Duration,
                Budget = dto.Budget,
                Skill = JsonSerializer.Serialize(dto.Skill),
                Paid = dto.Paid,
            };

            _context.Jobs.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new {
                message = "success",
                project = project,
            });
        }

        [Authorize]
        [HttpGet("get-client-jobs")]
        public async Task<IActionResult> ClientJob([FromQuery] string filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var query = _context.Jobs.Where(j => j.UserId == userId);


            if (filter == "jobs completed")
            {
                query = query.Where(j => j.Status == "completed");
            }else if (filter == "jobs in progress")
            {
                query = query.Where(j => j.Status == "open");
            }
            
            var jobs = await query.ToListAsync();

            return Ok(new {
                message = "success",
                jobs = jobs
            });
        }

        [Authorize]
        [HttpGet("get-reviews")]

        public async Task<IActionResult> ClientReviews()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var reviews = await _context.Reviews.Where(j => j.ReviewedId == userId).ToListAsync();

            return Ok(new {
                message = "success",
                reviews = reviews
            });
        }
    }
}
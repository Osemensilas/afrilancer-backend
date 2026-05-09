using Afrilancer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Afrilancer.DTOs;
using System.Text.Json;

namespace Afrilancer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ProfileController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("get-client-details")]
        public async Task<IActionResult> GetClientDetails()
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

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var skills = _context.Skills.FirstOrDefault(u => u.UserId == userId);

            return Ok(new {
                message = "success",
                client = user,
                skills = skills
            });
        }

        [Authorize]
        [HttpPost("add-phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] PhoneDto dto)
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

            var skillDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);

            if (skillDb != null)
            {
                skillDb.Phone = dto.Phone;
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                status = "success",
                message = "updated successfully",
                dto = dto
            });
        }

        [Authorize]
        [HttpGet("get-user-projects")]
        public async Task<IActionResult> GetUserProjects()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {
                    status = "error",
                    message = "Invalid request"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new {
                    status = "error",
                    message = "Unautorized user"
                });
            }

            var projectDb = await _context.Projects.Where(u => u.UserId == userId).OrderByDescending(p => p.Id).ToListAsync();

            if (projectDb == null)
            {
                return Ok(new
                {
                    status = "success",
                    message = "No project for user yet",
                });
            }

            foreach (var project in projectDb)
            {
                if (!string.IsNullOrEmpty(project.Images) && !project.Images.Trim().StartsWith("["))
                {
                    var imageList = project.Images
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(img => img.Trim())
                        .ToList();

                    project.Images = JsonSerializer.Serialize(imageList);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                status = "success",
                message = projectDb.Count == 0 ? "No project for user yet" : "Projects retrieved successfully",
                projects = projectDb
            });
        }
    }
}
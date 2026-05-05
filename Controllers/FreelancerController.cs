using Afrilancer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Afrilancer.DTOs;

namespace Afrilancer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FreelancerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public FreelancerController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("get-job-post")]
        public async Task<IActionResult> GetJobPost([FromQuery] string filter)
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

            var query = _context.Jobs;

            if (filter == "best fit")
            {
                
            }else if (filter == "saved")
            {
                
            }

            var jobs = await query.ToListAsync();

            return Ok(new {
                message = "success",
                jobs = jobs,
                filter = filter,
            });
        }

        [Authorize]
        [HttpPost("add-skills")]
        public async Task<IActionResult> AddNewSkill([FromBody] SkillDto dto)
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

            var message = "";
            var existingSkillsDb = _context.Skills.FirstOrDefault(u => u.UserId == userId);

            if (existingSkillsDb == null)
            {
                message = "Database do not exist";
            }
            else
            {
                message = "Database already exist";
            }

            return Ok(new
            {
                message = message,
                dto = dto.Skill
            });
        }
    }
}
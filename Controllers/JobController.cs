using Afrilancer.DTOs;
using Afrilancer.Models;
using Afrilancer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Afrilancer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class JobController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public JobController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [Authorize]
        [HttpGet("get-job-details")]
        public async Task<IActionResult> GetJobDetails([FromQuery] string job_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Invalid request"
                });    
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new
                {
                    status = "error",
                    message = "Unauthorized User"
                });
            }

            var jobId = job_id;

            var job = await _context.Jobs.SingleOrDefaultAsync(x => x.JobId == jobId);

            if (job.Coin == null)
            {
                int coin = 0;

                if (job.Budget < 50000)
                {
                    coin = 10;
                }
                else if (job.Budget >= 50000 && job.Budget < 100000)
                {
                    coin = 20;
                }
                else
                {
                    coin = 30;
                }

                job.Coin = coin;

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                status = "success",
                message = "Job retrieved successfully",
                job = job
            });
        }
    }
}
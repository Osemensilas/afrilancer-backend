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

            var userSkillsDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);

            var skillList = new List<string>();

            if (userSkillsDb != null && !string.IsNullOrEmpty(userSkillsDb.Skills))
            {
                skillList = JsonSerializer.Deserialize<List<string>>(userSkillsDb.Skills) ?? new List<string>();
            }

            var jobs = await _context.Jobs.OrderByDescending(j => j.Id).ToListAsync();

            if (filter == "best fit")
            {
                var rankedJobs = jobs.Select(job =>
                    {
                        var jobSkills = string.IsNullOrEmpty(job.Skill)? new List<string>(): JsonSerializer.Deserialize<List<string>>(job.Skill) ?? new List<string>();

                        var matchCount = jobSkills.Count(js =>
                            skillList.Any(us => us.Equals(js, StringComparison.OrdinalIgnoreCase))
                        );

                        return new
                        {
                            job,
                            matchCount
                        };
                    }).Where(x => x.matchCount > 0).OrderByDescending(x => x.matchCount).ThenByDescending(x => x.job.CreatedAt).Select(x => x.job).ToList();

                return Ok(new
                {
                    message = "success",
                    jobs = rankedJobs,
                    filter = filter,
                    userSkills = skillList
                });
            }
            else if (filter == "applied")
            {
                var appliedJobs = await _context.Applieds.Where(a => a.UserId == userId).OrderByDescending(a => a.Id).ToListAsync();

                if (appliedJobs.Count == 0)
                {
                    return Ok(new
                    {
                        message = "success",
                        jobs = new List<Job>(),
                        userSkills = skillList
                    });
                }
                
                var jobIds = appliedJobs.Select(a => a.JobId).ToList();

                var appliedJobsResult = await _context.Jobs.Where(j => jobIds.Contains(j.JobId)).OrderByDescending(j => j.Id).ToListAsync();

                return Ok(new
                {
                    message = "success",
                    jobs = appliedJobsResult,
                    userSkills = skillList
                });
            }

            return Ok(new
            {
                message = "success",
                jobs = jobs,
                filter = filter,
                userSkills = skillList
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

            var existingSkillsDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);

            if (existingSkillsDb == null)
            {
                var userSkill = new Skill
                {
                    UserId = userId,
                    Skills = JsonSerializer.Serialize(dto.Skill),
                };
                _context.Skills.Add(userSkill);
                await _context.SaveChangesAsync();
                message = "Skill added successfully";
            }
            else
            {
                var skillList = JsonSerializer.Deserialize<List<string>>(existingSkillsDb.Skills) ?? new List<string>();
                
                var newSkills = dto.Skill;

                foreach (var skill in newSkills)
                {
                    if (!skillList.Any(s => s.ToLower() == skill.ToLower()))
                    {
                        skillList.Add(skill);
                    }
                }

                existingSkillsDb.Skills = JsonSerializer.Serialize(skillList);
                await _context.SaveChangesAsync();

                message = "Skills processed successfully";
            }

            return Ok(new
            {
                status = "success",
                message = message,
            });
        }

        [Authorize]
        [HttpGet("fetch-skills")]
        public async Task<IActionResult> FetchSkills()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Reequets");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var userSkillsDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);

            var skillList = new List<string>();

            if (userSkillsDb != null && !string.IsNullOrEmpty(userSkillsDb.Skills))
            {
                skillList = JsonSerializer.Deserialize<List<string>>(userSkillsDb.Skills) ?? new List<string>();
            }

            return Ok(new
            {
                status = "success",
                message = "Data retrieved",
                skillList = skillList
            });
        }

        [Authorize]
        [HttpGet("remove-skills")]
        public async Task<IActionResult> RemoveSkills([FromQuery] string skill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Reequets");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var existingSkillsDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);

            var skillList = JsonSerializer.Deserialize<List<string>>(existingSkillsDb!.Skills) ?? new List<string>();

            var skillToRemove = skillList.FirstOrDefault(s => s.Equals(skill, StringComparison.OrdinalIgnoreCase));

            if (skillToRemove != null)
            {
                skillList.Remove(skillToRemove);

                existingSkillsDb.Skills = JsonSerializer.Serialize(skillList);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Skill removed successfully"
                });
            }
            
            return Ok(new
            {
                status = "success",
                message = "",
            });
        }

        [Authorize]
        [HttpPost("add-about")]
        public async Task<IActionResult> AddAbout([FromBody] AboutDto dto)
        {
            if (!ModelState.IsValid)
            {
                
                return BadRequest("Invalid Reequets");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var userSkillsDb = await _context.Skills.FirstOrDefaultAsync(u => u.UserId == userId);
            var message = "";

            if (userSkillsDb == null)
            {
                var userSkill = new Skill
                {
                    UserId = userId,
                    About = dto.About,
                };
                _context.Skills.Add(userSkill);
                await _context.SaveChangesAsync();
                message = "about added successfully";
            }
            else
            {
                userSkillsDb.About = dto.About;
                await _context.SaveChangesAsync();
                message = "About updated successfully";
            }
            
            return Ok(new
            {
                status = "success",
                message = message,
            });
        }

        [Authorize]
        [HttpPost("add-new-project")]
        public async Task<IActionResult> AddProject([FromForm] NewProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad Request"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new
                {
                    status = "error",
                    message = "Unathorized user"
                });
            }

            if (dto.Images == null || dto.Images.Count == 0)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "No images uploaded"
                });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/projects");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var imageNames = new List<string>();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            foreach (var image in dto.Images)
            {
                var extension = Path.GetExtension(image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = $"File type {extension} is not allowed"
                    });
                }

                if (image.Length > 1 * 1024 * 1024)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = $"Image {image.FileName} is larger than 1MB"
                    });
                }

                var uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imageNames.Add(uniqueName);
            }

            string projectId;

            do
            { 
                projectId = $"PROJ-{Guid.NewGuid().ToString("N").Substring(0,12).ToUpper()}";
            } 
            while (await _context.Projects.AnyAsync(p => p.ProjectId == projectId));

            var project = new Project
            {
                UserId = userId,
                ProjectId = projectId,
                Title = dto.Title,
                StartedAt = DateTime.SpecifyKind(dto.StartedAt, DateTimeKind.Utc),
                EndedAt = DateTime.SpecifyKind(dto.EndedAt, DateTimeKind.Utc),
                Description = dto.Description,
                Role = dto.Role,
                Images = JsonSerializer.Serialize(imageNames)
            };

            try
            {
                 _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Project added successfully" ,
                    project = project
                });
            }catch(Exception ex){
                return StatusCode(500, new
                {
                    status = "error",
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [Authorize]
        [HttpPost("delete-project")]
        public async Task<IActionResult> DeleteProject([FromBody] DeleteProjectDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad Request"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized(new
                {
                    status = "error",
                    message = "Unathorized user"
                });
            }

            var projectDb = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == dto.ProjectId && p.UserId == userId);

            if (projectDb == null)
            {
                return NotFound(new
                {
                    status = "error",
                    message = "Project not found"
                });
            }

            if (!string.IsNullOrEmpty(projectDb.Images))
            {
                var images = JsonSerializer.Deserialize<List<string>>(projectDb.Images);

                if (images != null)
                {
                    foreach (var image in images)
                    {
                        var imagePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/projects",
                            image
                        );

                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }
            }

            _context.Projects.Remove(projectDb);
            await _context.SaveChangesAsync();

            return Ok(new {
                status = "success",
                message = "message deleted successfully"
            });
        }

        [Authorize]
        [HttpGet("get-client-reviews")]
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

            var clientId = job?.UserId;

            var reviews = await _context.Reviews.Where(r => r.ReviewedId == userId).OrderByDescending(p => p.Id).ToListAsync();

            return Ok(new
            {
                status = "success",
                message = "Reviews retrieved successfully",
                reviews = reviews
            });
        }

        [Authorize]
        [HttpGet("get-freelance-coin")]
        public async Task<IActionResult> GetFreelanceCoins(){
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

            var coinDb = await _context.Coins.FirstOrDefaultAsync(x => x.UserId == userId);

            if (coinDb == null)
            {
                coinDb = new Coin
                {
                    UserId = userId,
                    Amount = 1000
                };

                _context.Coins.Add(coinDb);
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                status = "success",
                message = "Coins retrieved successfully",
                coins = coinDb.Amount
            });
        }
    }
}
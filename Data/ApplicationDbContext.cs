using Microsoft.EntityFrameworkCore;
using Afrilancer.Models;

namespace Afrilancer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
    }
}
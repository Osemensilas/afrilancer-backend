using Microsoft.EntityFrameworkCore;
using Afrilancer.Models;

namespace Afrilancer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Applied> Applieds { get; set; }
        public DbSet<Project> Projects { get; set; }
    }
}
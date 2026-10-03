using GymManagment.Configurations;
using GymManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.DbContexts
{
    public class GymManagmentDbContext : DbContext
    {
        public GymManagmentDbContext(DbContextOptions<GymManagmentDbContext> options) : base(options)
        {
            
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;Database=GymManagmentDB;Trusted_Connection=true;TrustServerCertificate=true");
        //}

        public DbSet<Plan> Plans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlanConfiguration());
            
        }
    }
}

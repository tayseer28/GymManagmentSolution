using GymManagment.DAL.Data.DataSeeding;
using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.PL
{
    public static class ProgramExtensions
    {
        public static async Task MigrateAndSeetDatabaseAsync(this WebApplication app)
        {
            using var scoop = app.Services.CreateScope();

            var dbContext = scoop.ServiceProvider.GetRequiredService<GymManagmentDbContext>();
            var logger = scoop.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var roleManager = scoop.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scoop.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
                await dbContext.Database.MigrateAsync();
            }
            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);
            await IdentityDataSeeding.SeedAsync(roleManager, userManager, logger);
        }
    }
}

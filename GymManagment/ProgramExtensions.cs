using GymManagment.DAL.Data.DataSeeding;
using GymManagment.DAL.Data.DbContexts;
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

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
                await dbContext.Database.MigrateAsync();
            }
            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "File");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);
        }
    }
}

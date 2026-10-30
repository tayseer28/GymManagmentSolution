using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GymManagment.DAL.Data.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymManagmentDbContext dbContent, string SeedFolderPath, ILogger logger, CancellationToken ct = default)
        {
			try
			{
				if( ! await dbContent.Plans.AnyAsync(ct))
				{
                    var plans = LoadDataFromJsonFile<Plan>(SeedFolderPath, "plans.json");
                    if(plans.Any())
                    {
                        dbContent.Plans.AddRange(plans);
                        logger.LogInformation($"Plans Seeded With Count = {plans.Count}");

                    }
                    if (dbContent.ChangeTracker.HasChanges())
                        await dbContent.SaveChangesAsync();
                    else
                        logger.LogInformation("Plans Already Seeded");
				}
			}
			catch (Exception ex)
			{

                logger.LogError(ex, "GymData Seeding Failed");
                throw;
			}
        }
		public static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
		{
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed Data File not Exist : {filePath}");

            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}

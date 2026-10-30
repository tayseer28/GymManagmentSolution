using GymManagment.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagment.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = await userManager.Users.AnyAsync(ct);
                bool HasRoles = await roleManager.Roles.AnyAsync(ct);

                if (HasUsers && HasRoles) return;

                var Roles = new List<IdentityRole>()
                    {
                        new IdentityRole("SuperAdmin"),
                        new IdentityRole("Admin")
                    };

                foreach (var role in Roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                            logger.LogError("Failed to create role {Role}: {Errors}", role.Name,
                                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    }
                }

                if (!HasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Aliaa",
                        LastName = "Tarek",
                        UserName = "AliaaTarek",
                        Email = "Aliaatarek@gmail.com",
                        PhoneNumber = "01123652635"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin01 = new ApplicationUser()
                    {
                        FirstName = "Omar",
                        LastName = "Mohamed",
                        UserName = "OmarMohamed",
                        Email = "OmarMohamed@gmail.com",
                        PhoneNumber = "01232589652"
                    };

                    await userManager.CreateAsync(Admin01, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin01, "Admin");

                    logger.LogInformation($"Identity Data Seeded");

                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity seeding failed.");
                return;
            }
        }

    }
}

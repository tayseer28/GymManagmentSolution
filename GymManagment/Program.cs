using GymManagment.BLL;
using GymManagment.BLL.Attachment;
using GymManagment.BLL.Services.Classes;
using GymManagment.BLL.Services.Interfaces;
using GymManagment.DAL.Data.DataSeeding;
using GymManagment.DAL.Data.DbContexts;
using GymManagment.DAL.Data.Models;
using GymManagment.DAL.Repositories.Classes;
using GymManagment.DAL.Repositories.Interfaces;
using GymManagment.PL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagment
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            #region Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();
            builder.Services.AddScoped<ISessionService, SessionService>(); 
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            #endregion

            #region Repositories _ UOW
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion;

            builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
            builder.Services.AddDbContext<GymManagmentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Config =>
            {
                //Config.Password.RequiredLength = 6;               // default
                //Config.Password.RequireLowercase = true;          // default
                //Config.Password.RequireUppercase = true;          // default
                Config.User.RequireUniqueEmail = true;
                Config.Lockout.MaxFailedAccessAttempts = 5;
                Config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

            }).AddEntityFrameworkStores<GymManagmentDbContext>();



            var app = builder.Build();

            await app.MigrateAndSeetDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}

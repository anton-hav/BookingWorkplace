using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using BookingWorkplace.Business.ServiceImplementations;
using BookingWorkplace.Core.Abstractions;
using BookingWorkplace.Data.Abstractions;
using BookingWorkplace.Data.Abstractions.Repositories;
using BookingWorkplace.Data.Repositories;
using BookingWorkplace.DataBase;
using BookingWorkplace.DataBase.Entities;
using BookingWorkplace.IdentityManagers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BookingWorkplace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .WriteTo.File(GetPathToLogFile(),
                    LogEventLevel.Information));

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<BookingWorkplaceDbContext>(
                optionBuilder => optionBuilder.UseSqlServer(connectionString));

            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.LoginPath = new PathString(@"/Account/Login");
                    options.LogoutPath = new PathString(@"/Account/Logout");
                    options.AccessDeniedPath = new PathString(@"/Account/Login");
                });

            builder.Services.AddHttpContextAccessor();

            // Add business services
            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddScoped<IWorkplaceService, WorkplaceService>();
            builder.Services.AddScoped<IEquipmentForWorkplaceService, EquipmentForWorkplaceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            // Add custom identity services
            builder.Services.AddScoped<ISignInManager, SignInManager>();
            builder.Services.AddScoped<IUserManager, UserManager>();

            // Add repositories
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Equipment>, Repository<Equipment>>();
            builder.Services.AddScoped<IRepository<EquipmentForWorkplace>, Repository<EquipmentForWorkplace>>();
            builder.Services.AddScoped<IRepository<Reservation>, Repository<Reservation>>();
            builder.Services.AddScoped<IRepository<Workplace>, Repository<Workplace>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Configuration.AddJsonFile("secrets.json");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        /// <summary>
        /// Returns the path for log file recording.
        /// </summary>
        /// <returns>A string whose value contains a path to the log file</returns>
        private static string GetPathToLogFile()
        {
            var sb = new StringBuilder();
            sb.Append(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            sb.Append(@"\logs\");
            sb.Append($"{DateTime.Now:yyyyMMddhhmmss}");
            sb.Append("data.log");
            return sb.ToString();
        }
    }
}
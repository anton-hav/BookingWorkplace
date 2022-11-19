using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using BookingWorkplace.Data.Abstractions.Repositories;
using BookingWorkplace.Data.Repositories;
using BookingWorkplace.DataBase;
using BookingWorkplace.DataBase.Entities;
using Microsoft.EntityFrameworkCore;

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

            // Add repositories
            builder.Services.AddScoped<IRepository<Employee>, Repository<Employee>>();
            builder.Services.AddScoped<IRepository<Equipment>, Repository<Equipment>>();
            builder.Services.AddScoped<IRepository<EquipmentForWorkplace>, Repository<EquipmentForWorkplace>>();
            builder.Services.AddScoped<IRepository<Reservation>, Repository<Reservation>>();
            builder.Services.AddScoped<IRepository<Room>, Repository<Room>>();
            builder.Services.AddScoped<IRepository<Workplace>, Repository<Workplace>>();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        /// <summary>
        ///     Returns the path for log file recording.
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Seeding;

namespace NutriWeek
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<NutriWeekDbContext>(options =>
                options.UseSqlServer(builder.Configuration
                .GetConnectionString("SqlServerDev") ??
                throw new InvalidOperationException("Connection string is not valid!")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<NutriWeekDbContext>();
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(
                    new AutoValidateAntiforgeryTokenAttribute());
            });
            builder.Services.AddApplicationServices();
            var app = builder.Build();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                NutriWeekDbContext dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<NutriWeekDbContext>();


                SharedDishIngredientsSeeder
                    .SeedAsync(dbContext)
                    .GetAwaiter()
                    .GetResult();


                IdentitySeeder
                    .SeedRoles(
                        scope.ServiceProvider);


                string adminEmail =
                    app.Configuration[
                        "UserSeed:AdminAccount:Email"] ??
                    throw new InvalidOperationException(
                        "Administrator email is not configured.");

                string adminPassword =
                    app.Configuration[
                        "UserSeed:AdminAccount:Password"] ??
                    throw new InvalidOperationException(
                        "Administrator password is not configured.");


                IdentitySeeder
                    .SeedAdminUser(
                        scope.ServiceProvider,
                        adminEmail,
                        adminPassword);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
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
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data
{
    public class NutriWeekDbContext : DbContext
    {

        public NutriWeekDbContext(DbContextOptions<NutriWeekDbContext> options) : base(options)
        {
            
        }

        public DbSet<Dish> Dishes { get; set; } = null!;    

        public DbSet<WeeklyMenu> WeeklyMenus { get; set; } = null!;

        public DbSet<DailyMenu> DailyMenus { get; set; } = null!;

        public DbSet<Meal> Meals { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder builder)
        {
           builder.ApplyConfigurationsFromAssembly(typeof(NutriWeekDbContext).Assembly);

        }
    }
}

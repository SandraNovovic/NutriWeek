using Microsoft.EntityFrameworkCore;
using NutriWeek.Data.Models;

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
    }
}

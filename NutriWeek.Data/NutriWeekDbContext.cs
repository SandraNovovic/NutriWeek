using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data.Models;

namespace NutriWeek.Data
{
    public class NutriWeekDbContext : IdentityDbContext
    {

        public NutriWeekDbContext(DbContextOptions<NutriWeekDbContext> options) : base(options)
        {

        }

        public DbSet<Dish> Dishes { get; set; } = null!;

        public DbSet<WeeklyMenu> WeeklyMenus { get; set; } = null!;

        public DbSet<DailyMenu> DailyMenus { get; set; } = null!;

        public DbSet<Meal> Meals { get; set; } = null!;

        public DbSet<Ingredient> Ingredients { get; set; } = null!;

        public DbSet<DishIngredient> DishIngredients { get; set; } = null!;

        public DbSet<PantryItem> PantryItems { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(NutriWeekDbContext).Assembly);

        }

    }
}

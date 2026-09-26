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
            base.OnModelCreating(builder);

            builder.Entity<Dish>()
                .HasData(
                    new Dish
                    {
                        Id = 1,
                        Name = "Chicken Rice Bowl",
                        Description = "A balanced meal with chicken, rice and vegetables.",
                        Ingredients = "Chicken breast, rice, tomato, cucumber, olive oil",
                        Instructions = "Cook the rice. Cook the chicken. Chop the vegetables and serve everything together.",
                        Calories = 520,
                        PreparationTime = 30,
                        Portions = 2,
                        ImageUrl = null,
                        DishType = DishType.Meat
                    },
                    new Dish
                    {
                        Id = 2,
                        Name = "Vegetarian Pasta",
                        Description = "Pasta with vegetables and tomato sauce.",
                        Ingredients = "Pasta, tomato sauce, zucchini, bell pepper, parmesan",
                        Instructions = "Cook the pasta. Cook the vegetables and tomato sauce. Mix everything together.",
                        Calories = 430,
                        PreparationTime = 25,
                        Portions = 2,
                        ImageUrl = null,
                        DishType = DishType.Vegetarian
                    },
                    new Dish
                    {
                        Id = 3,
                        Name = "Salmon with Vegetables",
                        Description = "Baked salmon served with fresh vegetables.",
                        Ingredients = "Salmon, broccoli, carrots, olive oil, lemon",
                        Instructions = "Season the salmon and vegetables. Bake until cooked and serve with lemon.",
                        Calories = 480,
                        PreparationTime = 35,
                        Portions = 2,
                        ImageUrl = null,
                        DishType = DishType.Fish
                    },
                    new Dish
                    {
                        Id = 4,
                        Name = "Vegan Oatmeal",
                        Description = "Oatmeal with banana and fresh berries.",
                        Ingredients = "Oats, oat milk, banana, blueberries, strawberries",
                        Instructions = "Cook the oats with oat milk. Add sliced banana and berries before serving.",
                        Calories = 350,
                        PreparationTime = 10,
                        Portions = 1,
                        ImageUrl = null,
                        DishType = DishType.Vegan
                    }
                );

            builder.Entity<WeeklyMenu>()
               .HasData(
        new WeeklyMenu
        {
            Id = 1,
            Title = "Healthy Week",
            WeekStartDate = new DateTime(2026, 9, 21)
        }
    );

            builder.Entity<DailyMenu>()
             .HasData(
                new DailyMenu
                {
                    Id = 1,
                    Date = new DateTime(2026, 9, 21),
                    WeeklyMenuId = 1
                },
                new DailyMenu
                {
                    Id = 2,
                    Date = new DateTime(2026, 9, 22),
                    WeeklyMenuId = 1
                }
    );

            builder.Entity<Meal>()
              .HasData(
                  new Meal
                  {
                      Id = 1,
                      MealType = MealType.Breakfast,
                      DishId = 4,
                      DailyMenuId = 1
                  },
                  new Meal
                  {
                      Id = 2,
                      MealType = MealType.Lunch,
                      DishId = 1,
                      DailyMenuId = 1
                  },
                  new Meal
                  {
                      Id = 3,
                      MealType = MealType.Dinner,
                      DishId = 3,
                      DailyMenuId = 1
                  },
              
                  new Meal
                  {
                      Id = 4,
                      MealType = MealType.Breakfast,
                      DishId = 4,
                      DailyMenuId = 2
                  },
                  new Meal
                  {
                      Id = 5,
                      MealType = MealType.Lunch,
                      DishId = 2,
                      DailyMenuId = 2
                  },
                  new Meal
                  {
                      Id = 6,
                      MealType = MealType.Dinner,
                      DishId = 1,
                      DailyMenuId = 2
                  }
              );
        }
    }
}

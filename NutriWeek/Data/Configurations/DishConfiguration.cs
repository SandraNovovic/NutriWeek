using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data.Configurations
{
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public readonly IEnumerable<Dish> seedDishes = new List<Dish>
        {
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
        };

        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.HasData(seedDishes);
        }
    }
}

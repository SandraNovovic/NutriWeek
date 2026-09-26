using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data.Configurations
{
    public class MealConfiguration
    {
        public readonly IEnumerable<Meal> seedMeals = new List<Meal>
        {
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
        };

        public void Configure(EntityTypeBuilder<Meal> builder)
        {
            builder.HasData(seedMeals);
        }
    }
}

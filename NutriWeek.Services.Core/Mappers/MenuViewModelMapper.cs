using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels.DailyMenus;

namespace NutriWeek.Services.Core.Mappers
{
    internal static class MenuViewModelMapper
    {
        public static DailyMenuIndexViewModel MapDailyMenu(DailyMenu dailyMenu)
        {
            return new DailyMenuIndexViewModel
            {
                Date = dailyMenu.Date,
                Calories = dailyMenu.Meals.Sum(m => m.Dish.Calories),
                PreparationTime = dailyMenu.Meals.Sum(m => m.Dish.PreparationTime),
                TotalMeals = dailyMenu.Meals.Count,
                Portions = dailyMenu.Meals.Sum(m => m.Dish.Portions),
                BreakfastMeals = MapMeals(dailyMenu.Meals, MealType.Breakfast),
                LunchMeals = MapMeals(dailyMenu.Meals, MealType.Lunch),
                DinnerMeals = MapMeals(dailyMenu.Meals, MealType.Dinner)
            };
        }

        private static List<DailyMenuMealViewModel> MapMeals(
            IEnumerable<Meal> meals,
            MealType mealType)
        {
            return meals
                .Where(m => m.MealType == mealType)
                .Select(MapMeal)
                .ToList();
        }

        private static DailyMenuMealViewModel MapMeal(Meal meal)
        {
            return new DailyMenuMealViewModel
            {
                MealId = meal.Id,
                DishId = meal.DishId,
                DishName = meal.Dish.Name,
                Description = meal.Dish.Description,
                DishType = meal.Dish.DishType.ToString(),
                MealType = meal.MealType.ToString(),
                Calories = meal.Dish.Calories,
                PreparationTime = meal.Dish.PreparationTime,
                Portions = meal.Dish.Portions,
                ImageUrl = meal.Dish.ImageUrl
            };
        }
    }
}

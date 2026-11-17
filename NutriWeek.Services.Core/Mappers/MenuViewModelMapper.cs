using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;

using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;

namespace NutriWeek.Web.Mappers
{
    public static class MenuViewModelMapper
    {
        public static WeeklyMenuIndexViewModel MapWeeklyMenu( WeeklyMenuResponseModel response)
        {
            return new WeeklyMenuIndexViewModel
            {
                Id = response.Id,
                StartDate = response.StartDate,
                TotalCalories = response.TotalCalories,
                TotalPreparationTime = response.TotalPreparationTime,
                TotalMeals = response.TotalMeals,
                TotalPortions = response.TotalPortions,

                DailyMenus =
                    response.DailyMenus
                        .Select(MapDailyMenu)
                        .ToList()
            };
        }


        public static DailyMenuIndexViewModel MapDailyMenu( GetAllDailyMenusResponseModel response)
        {
            return new DailyMenuIndexViewModel
            {
                Date = response.Date,
                Calories = response.Calories,
                PreparationTime = response.PreparationTime,
                TotalMeals = response.TotalMeals,
                Portions = response.Portions,

                BreakfastMeals =
                    response.BreakfastMeals
                        .Select(MapMeal)
                        .ToList(),

                LunchMeals =
                    response.LunchMeals
                        .Select(MapMeal)
                        .ToList(),

                DinnerMeals =
                    response.DinnerMeals
                        .Select(MapMeal)
                        .ToList()
            };
        }


        public static DailyMenuMealViewModel MapMeal( DailyMenuMealResponseModel response)
        {
            return new DailyMenuMealViewModel
            {
                MealId = response.MealId,
                DishId = response.DishId,
                DishName = response.DishName,
                Description = response.Description,
                DishType = response.DishType,
                MealType = response.MealType,
                Calories = response.Calories,
                PreparationTime = response.PreparationTime,
                Portions = response.Portions,
                ImageUrl = response.ImageUrl
            };
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels.DailyMenus;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class DailyMenusController : Controller
    {
        private readonly IDailyMenuService _dailyMenuService;


        public DailyMenusController( IDailyMenuService dailyMenuService)
        {
            _dailyMenuService = dailyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            List<GetAllDailyMenusResponseModel> response = await _dailyMenuService .GetAllAsync(userId);


            List<DailyMenuIndexViewModel> dailyMenus = response
                    .Select(MapDailyMenu)
                    .ToList();


            return View(dailyMenus);
        }


        private static DailyMenuIndexViewModel MapDailyMenu( GetAllDailyMenusResponseModel response)
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


        private static DailyMenuMealViewModel MapMeal(DailyMenuMealResponseModel response)
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
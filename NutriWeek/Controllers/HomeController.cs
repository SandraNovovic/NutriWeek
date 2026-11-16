using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.ViewModels.Home;
using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IWeeklyMenuService _weeklyMenuService;


        public HomeController( IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService =weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            WeeklyMenuResponseModel? response =
                await _weeklyMenuService
                    .GetCurrentWeekAsync(
                        userId);


            WeeklyMenuIndexViewModel? currentWeek =
                response == null
                    ? null
                    : MapWeeklyMenu(response);


            HomeIndexViewModel model =
                new HomeIndexViewModel
                {
                    CurrentWeek =
                        currentWeek
                };


            return View(model);
        }


        private static WeeklyMenuIndexViewModel MapWeeklyMenu( WeeklyMenuResponseModel response)
        {
            return new WeeklyMenuIndexViewModel
            {
                Id =
                    response.Id,

                StartDate =
                    response.StartDate,

                TotalCalories =
                    response.TotalCalories,

                TotalPreparationTime =
                    response.TotalPreparationTime,

                TotalMeals =
                    response.TotalMeals,

                TotalPortions =
                    response.TotalPortions,

                DailyMenus =
                    response.DailyMenus
                        .Select(MapDailyMenu)
                        .ToList()
            };
        }


        private static DailyMenuIndexViewModel MapDailyMenu( GetAllDailyMenusResponseModel response)
        {
            return new DailyMenuIndexViewModel
            {

                Date =
                    response.Date,

                Calories =
                    response.Calories,

                PreparationTime =
                    response.PreparationTime,

                TotalMeals =
                    response.TotalMeals,

                Portions =
                    response.Portions,

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
                MealId =
                    response.MealId,

                DishId =
                    response.DishId,

                DishName =
                    response.DishName,

                Description =
                    response.Description,

                DishType =
                    response.DishType,

                MealType =
                    response.MealType,

                Calories =
                    response.Calories,

                PreparationTime =
                    response.PreparationTime,

                Portions =
                    response.Portions,

                ImageUrl =
                    response.ImageUrl
            };
        }
    }
}
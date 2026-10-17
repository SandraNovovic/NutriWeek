using Microsoft.AspNetCore.Mvc;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.Meals;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    public class MealsController : Controller
    {
        private readonly IMealService _mealService;

        public MealsController(IMealService mealService)
        {
            _mealService = mealService;
            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<MealIndexViewModel> meals =
                await _mealService.GetAllAsync();

            return View(meals);
        }
    }
}

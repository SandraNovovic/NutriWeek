using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data.Models.Enums;
using NutriWeek.GCommon;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.Meals;
using System.Security.Claims;
using static NutriWeek.GCommon.TempDateMessages;
namespace NutriWeek.Web.Controllers
{
    public class MealsController : Controller
    {
        private readonly IMealService _mealService;
        private readonly ILogger<MealsController> _logger;
        public MealsController(IMealService mealService, ILogger<MealsController> logger)
        {
            _mealService = mealService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<MealIndexViewModel> meals =
                await _mealService.GetAllAsync();

            return View(meals);
        }

        [HttpGet]
        public async Task<IActionResult> Add(DateTime? date, MealType? mealType)
        {
            AddMealViewModel dishes =
                new AddMealViewModel
                {
                    Dishes =
                        await _mealService
                            .LoadDishesAsync(),
                    MealTypes = _mealService.LoadMealTypes(),
                    Date =
                        date,
                    MealType=mealType ?? MealType.Breakfast

                };

            return View(dishes);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddMealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Dishes =
                    await _mealService
                        .LoadDishesAsync();

                model.MealTypes= _mealService.LoadMealTypes();

                return View(model);
            }


            bool dishExists =
                await _mealService
                    .DishExistsAsync( model.DishId);


            if (!dishExists)
            {
                ModelState.AddModelError(
                    nameof(model.DishId),
                    ErrorMessages.DishNotFound);


                model.Dishes =
                    await _mealService.LoadDishesAsync();

                model.MealTypes = _mealService.LoadMealTypes();

                return View(model);
            }


            try
            {
                bool dailyMenuCreated =
                    await _mealService.AddMealAsync(model);


                if (dailyMenuCreated)
                {
                    TempData["SuccessMessage"] =
                        AddDailyMenuSuccessMessage;
                }


                TempData["SuccessMessage"] =
                    AddMealSuccessMessage;
            }
            catch 
            {
                throw;
            }


            return RedirectToAction("Index");
        }
    }
}

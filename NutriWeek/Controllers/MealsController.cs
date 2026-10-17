using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data.Models.Enums;
using NutriWeek.GCommon;
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


        [HttpGet]
        public async Task<IActionResult> Add(DateTime? date,MealType? mealType)
        {
            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            AddMealViewModel dishes =
                new AddMealViewModel
                {
                    Dishes =
                        await _mealService
                            .LoadDishesAsync(),

                    Date =
                        date,

                    MealType = _mealService.LoadMealTypes()
                };


            return View(dishes);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddMealViewModel model)
        {
            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            if (!ModelState.IsValid)
            {
                model.Dishes =
                    await _mealService
                        .LoadDishesAsync(
                            userId);


                return View(model);
            }


            bool dishExists =
                await _mealService
                    .DishExistsAsync(
                        model.DishId,
                        userId);


            if (!dishExists)
            {
                ModelState.AddModelError(
                    nameof(model.DishId),
                    ErrorMessages.DishNotFound);


                model.Dishes =
                    await _mealService
                        .LoadDishesAsync(
                            userId);


                return View(model);
            }


            try
            {
                bool dailyMenuCreated =
                    await _mealService
                        .AddMealAsync(
                            model,
                            userId);


                if (dailyMenuCreated)
                {
                    TempData["SuccessMessage"] =
                        AddDailyMenuSuccessMessage;
                }


                TempData["SuccessMessage"] =
                    AddMealSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                model.Dishes =
                    await _mealService
                        .LoadDishesAsync(
                            userId);


                TempData["ErrorMessage"] =
                    AddMealErrorMessage;


                return View(model);
            }


            return RedirectToAction("Index");
        }
    }
}

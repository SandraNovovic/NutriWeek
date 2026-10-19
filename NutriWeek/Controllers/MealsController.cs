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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                model.Dishes =
                    await _mealService
                        .LoadDishesAsync();

                model.MealTypes = _mealService.LoadMealTypes();

                TempData["ErrorMessage"] =
                    AddMealErrorMessage;


                return View(model);
                throw;
            }


            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit( [FromRoute] int? id)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages.InvalidRequest);
            }


            AddMealViewModel? meal =
                await _mealService
                    .GetMealForEditAsync(id.Value);


            if (meal == null)
            {
                return NotFound(
                    ErrorMessages.MealNotFound);
            }


            return View(meal);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int? id,AddMealViewModel meal)
        {

            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest();
            }


            if (!ModelState.IsValid)
            {
                meal.Dishes =
                    await _mealService
                        .LoadDishesAsync();


                return View(meal);
            }


            bool mealTypeExists =
                _mealService
                    .IsMealTypeValid(meal.MealType);


            if (!mealTypeExists)
            {
                ModelState.AddModelError(
                    nameof(meal.MealType),
                    ErrorMessages.InvalidMealType);


                meal.Dishes =
                    await _mealService
                        .LoadDishesAsync();


                return View(meal);
            }


            bool dishExists =
                await _mealService
                    .DishExistsAsync(meal.DishId);


            if (!dishExists)
            {
                ModelState.AddModelError(
                    nameof(meal.DishId),
                    ErrorMessages.DishNotFound);


                meal.Dishes =
                    await _mealService
                        .LoadDishesAsync();


                return View(meal);
            }


            try
            {
                bool edited =
                    await _mealService
                        .EditMealAsync(id.Value, meal);


                if (!edited)
                {
                    return NotFound();
                }


                TempData["Success"] =
                    UpdateMealSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                TempData["Error"] =
                    UpdateMealErrorMessage;


                meal.Dishes =
                    await _mealService
                        .LoadDishesAsync();


                return View(meal);
            }


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete( [FromRoute] int? id)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(ErrorMessages.InvalidRequestTryAgain);
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            DeleteMealViewModel? deletingMeal =
                await _mealService
                    .GetMealForDeleteAsync(id.Value);


            if (deletingMeal == null)
            {
                return NotFound(
                    ErrorMessages.MealNotFound);
            }


            return View(deletingMeal);
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int? id, DeleteMealViewModel meal)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages.InvalidRequestTryAgain);
            }

            try
            {
                bool deleted =
                    await _mealService
                        .DeleteMealAsync(id.Value);


                if (!deleted)
                {
                    return NotFound();
                }


                TempData["Success"] =
                    DeleteMealSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                TempData["Error"] =
                    DeleteMealErrorMessage;
            }

            return RedirectToAction(
                nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? id)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(ErrorMessages.InvalidRequest);
            }



            MealDetailsViewModel? meal =
                await _mealService
                    .GetMealDetailsAsync(id.Value);


            if (meal == null)
            {
                return NotFound( ErrorMessages.MealNotFound);
            }


            return View(meal);
        }


    }
}

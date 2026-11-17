using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Data.Models.Enums;
using NutriWeek.GCommon;

using NutriWeek.Services.Core.Data.Meal.Request;
using NutriWeek.Services.Core.Data.Meal.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Meals;

using System.Security.Claims;

using static NutriWeek.GCommon.TempDateMessages;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class MealsController : Controller
    {
        private readonly IMealService _mealService;
        private readonly ILogger<MealsController> _logger;


        public MealsController(IMealService mealService,  ILogger<MealsController> logger)
        {
            _mealService = mealService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =  User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            List<GetAllMealsResponseModel> meals = await _mealService
                    .GetAllAsync(userId);


            List<MealIndexViewModel> viewModels =
                meals
                    .Select(m =>
                        new MealIndexViewModel
                        {
                            Id = m.Id,
                            Date = m.Date,
                            DishName = m.DishName,
                            Calories = m.Calories,
                            Preparation = m.PreparationTime,
                            MealType = m.MealType
                        })
                    .ToList();


            return View(viewModels);
        }


        [HttpGet]
        public async Task<IActionResult> Add( DateTime? date,  MealType? mealType)
        {
            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            AddMealViewModel model =
                new AddMealViewModel
                {
                    Dishes =
                        await LoadDishesAsync(userId),

                    MealTypes =
                        LoadMealTypes(),

                    Date = date,

                    MealType =
                        mealType ??
                        MealType.Breakfast
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddMealViewModel model)
        {
            string userId =   User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(
                    model,
                    userId);

                return View(model);
            }


            CreateMealRequestData requestData =
                new CreateMealRequestData
                {
                    DishId = model.DishId,
                    Date = model.Date,
                    MealType = model.MealType
                };


            MealServiceStatusResponseModel response =
                await _mealService
                    .AddMealAsync(
                        requestData,
                        userId);


            if (!response.Status)
            {
                foreach (string? error
                         in response.ErrorMessages)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error ??
                        ErrorMessages.InvalidRequest);
                }


                await LoadDropdownDataAsync(
                    model,
                    userId);


                TempData["ErrorMessage"] =
                    AddMealErrorMessage;


                return View(model);
            }


            TempData["SuccessMessage"] =
                AddMealSuccessMessage;


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit( [FromRoute] int? id)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequest);
            }


            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            GetMealForEditResponseModel? response =
                await _mealService
                    .GetMealForEditAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound(
                    ErrorMessages.MealNotFound);
            }


            AddMealViewModel model =
                new AddMealViewModel
                {
                    DishId = response.DishId,
                    Date = response.Date,
                    MealType = response.MealType,

                    Dishes =
                        await LoadDishesAsync(userId),

                    MealTypes =
                        LoadMealTypes()
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int? id,AddMealViewModel model)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(  ErrorMessages.InvalidRequest);
            }


            string userId =   User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(
                    model,
                    userId);

                return View(model);
            }


            EditMealRequestData requestData =
                new EditMealRequestData
                {
                    DishId = model.DishId,
                    Date = model.Date,
                    MealType = model.MealType
                };


            MealServiceStatusResponseModel response =
                await _mealService
                    .EditMealAsync(
                        id.Value,
                        requestData,
                        userId);


            if (!response.Status)
            {
                foreach (string? error
                         in response.ErrorMessages)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error ??
                        ErrorMessages.InvalidRequest);
                }


                await LoadDropdownDataAsync(
                    model,
                    userId);


                TempData["Error"] =
                    UpdateMealErrorMessage;


                return View(model);
            }


            TempData["Success"] =
                UpdateMealSuccessMessage;


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete( [FromRoute] int? id)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages.InvalidRequestTryAgain);
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            GetMealByIdResponseModel? response =
                await _mealService
                    .GetMealForDeleteAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound(
                    ErrorMessages.MealNotFound);
            }


            DeleteMealViewModel model =
                new DeleteMealViewModel
                {
                    Id = response.Id,
                    DishName = response.DishName,
                    Date = response.Date,

                    MealType =
                        response.MealType
                            .ToString(),

                    ImageUrl = response.ImageUrl
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Delete( [FromRoute] int? id,  DeleteMealViewModel model, bool fromWeeklyMenu = false,string? weekStart = null)
        {
            if (!id.HasValue ||id.Value <= 0)
            {
                return BadRequest(  ErrorMessages.InvalidRequestTryAgain);
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            MealServiceStatusResponseModel response =
                await _mealService
                    .DeleteMealAsync(
                        id.Value,
                        userId);


            if (!response.Status)
            {
                TempData["Error"] =
                    DeleteMealErrorMessage;


                return NotFound(
                    ErrorMessages.MealNotFound);
            }


            TempData["Success"] =
                DeleteMealSuccessMessage;


            if (fromWeeklyMenu)
            {
                return RedirectToAction(
                    "Index",
                    "WeeklyMenus",
                    new
                    {
                        weekStart
                    });
            }


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? id)
        {
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequest);
            }


            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            GetMealDetailsResponseModel? response =
                await _mealService
                    .GetMealDetailsAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound( ErrorMessages.MealNotFound);
            }


            MealDetailsViewModel model =
                new MealDetailsViewModel
                {
                    Id = response.Id,
                    Date = response.Date,
                    DishName = response.DishName,
                    Description = response.Description,

                    DishType =
                        response.DishType
                            .ToString(),

                    MealType =
                        response.MealType
                            .ToString(),

                    Calories = response.Calories,
                    PreparationTime = response.PreparationTime,
                    Portions = response.Portions,
                    ImageUrl = response.ImageUrl
                };


            return View(model);
        }


        private async Task LoadDropdownDataAsync(  AddMealViewModel model, string userId)
        {
            model.Dishes = await LoadDishesAsync(userId);

            model.MealTypes = LoadMealTypes();
        }


        private async Task<IEnumerable<DropdownViewModel>> LoadDishesAsync(string userId)
        {
            return (await _mealService
                    .LoadAvailableDishesAsync(userId))
                .Select(x =>
                    new DropdownViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                .ToList();
        }


        private IEnumerable<DropdownViewModel> LoadMealTypes()
        {
            return _mealService
                .LoadMealTypes()
                .Select(x =>
                    new DropdownViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                .ToList();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.GCommon;

using NutriWeek.Services.Core.Data.Dish.Request;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dish;
using NutriWeek.Web.ViewModels.Dishes;

using System.Security.Claims;

using static NutriWeek.GCommon.TempDateMessages;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class DishesController : Controller
    {
        private readonly IDishService _dishService;
        private readonly ILogger<DishesController> _logger;


        public DishesController(
            IDishService dishService,
            ILogger<DishesController> logger)
        {
            _dishService = dishService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            NutriWeek.Data.Models.Enums.DishType? dishType,
            string? cookingTime,
            string? calories,
            string? sortBy,
            int page = 1)
        {
            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            GetDishesPageRequestData request =
                new GetDishesPageRequestData
                {
                    Search =
                        search,

                    DishType =
                        dishType,

                    CookingTime =
                        cookingTime,

                    Calories =
                        calories,

                    SortBy =
                        sortBy,

                    Page =
                        page,

                    PageSize =
                        8
                };


            GetDishesPageResponseModel response =
                await _dishService
                    .GetPagedAsync(
                        request,
                        userId);


            DishesIndexViewModel model =
                new DishesIndexViewModel
                {
                    Search =
                        search,

                    DishType =
                        dishType,

                    CookingTime =
                        cookingTime,

                    Calories =
                        calories,

                    SortBy =
                        sortBy,

                    CurrentPage =
                        response.CurrentPage,

                    TotalPages =
                        response.TotalPages,

                    TotalItems =
                        response.TotalItems,

                    VegetarianCount =
                        response.VegetarianCount,

                    VeganCount =
                        response.VeganCount,

                    MeatCount =
                        response.MeatCount,

                    FishCount =
                        response.FishCount,

                    Dishes =
                        response.Dishes
                            .Select(d =>
                                new DishIndexViewModel
                                {
                                    Id =
                                        d.Id,

                                    Name =
                                        d.Name,

                                    Description =
                                        d.Description,

                                    Calories =
                                        d.Calories,

                                    PreparationTime =
                                        d.PreparationTime,

                                    Portions =
                                        d.Portions,

                                    ImageUrl =
                                        d.ImageUrl,

                                    DishType =
                                        d.DishType,

                                    IsShared =
                                        d.IsShared
                                })
                            .ToList()
                };


            return View(model);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AddDishViewModel model =
                new AddDishViewModel
                {
                    DishTypes =
                        LoadDishTypes()
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Add( [FromForm] AddDishViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes =
                    LoadDishTypes();

                return View(model);
            }


            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;



            CreateDishRequestData request =
                new CreateDishRequestData
                {
                    Name = model.Name,
                    Description = model.Description,
                    Ingredients =
                        BuildIngredientsText(
                            model.IngredientItems),
                    Instructions = model.Instructions,
                    Calories = model.Calories,
                    PreparationTime = model.PreparationTime,
                    Portions = model.Portions,
                    ImageUrl = model.ImageUrl,
                    DishType = model.DishType,
                    IngredientItems = model.IngredientItems
                      .Select(i => new DishIngredientRequestData
                      {
                          Name = i.Name,
                          Quantity = i.Quantity,
                          Unit = i.Unit
                      })
                      .ToList()
                      };


            try
            {
                DishServiceStatusResponseModel response =
                    await _dishService
                        .AddDishAsync(
                            request,
                            userId);


                if (!response.Status)
                {
                    foreach (string? error
                             in response.ErrorMessages)
                    {
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            ModelState.AddModelError(
                                string.Empty,
                                error);
                        }
                    }


                    model.DishTypes =
                        LoadDishTypes();


                    return View(model);
                }


                TempData["SuccessMessage"] =
                    AddDishSuccessMessage;


                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                model.DishTypes =
                    LoadDishTypes();


                TempData["ErrorMessage"] =
                    AddDishErrorMessage;


                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit( [FromRoute] int? id)
        {
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequest);
            }


            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            GetDishForEditResponseModel? response =
                await _dishService
                    .GetDishForEditAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound(
                    ErrorMessages.DishNotFound);
            }


            AddDishViewModel model =
                new AddDishViewModel
                {
                    Name = response.Name,
                    Description = response.Description,
                    Ingredients = response.Ingredients,
                    Instructions = response.Instructions,
                    Calories = response.Calories,
                    PreparationTime = response.PreparationTime,
                    Portions = response.Portions,
                    ImageUrl = response.ImageUrl,
                    DishType = response.DishType,
                    IngredientItems = response.IngredientItems
                    .Select(i => new DishIngredientInputViewModel
                    {
                        Name = i.Name,
                        Quantity = i.Quantity,
                        Unit = i.Unit
                    })
                    .ToList(),
                    DishTypes =
                        LoadDishTypes()
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int? id, AddDishViewModel model)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequest);
            }


            if (!ModelState.IsValid)
            {
                model.DishTypes =
                    LoadDishTypes();

                return View(model);
            }


            bool dishTypeExists =
                _dishService
                    .IsDishTypeValid(
                        model.DishType);


            if (!dishTypeExists)
            {
                ModelState.AddModelError(
                    nameof(model.DishType),
                    ErrorMessages.InvalidDishType);


                model.DishTypes =
                    LoadDishTypes();


                return View(model);
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            EditDishRequestData request =
                new EditDishRequestData
                {
                    Name = model.Name,
                    Description = model.Description,
                    Ingredients =
                        BuildIngredientsText(
                            model.IngredientItems),
                    Instructions = model.Instructions,
                    Calories = model.Calories,
                    PreparationTime = model.PreparationTime,
                    Portions = model.Portions,
                    ImageUrl = model.ImageUrl,
                    DishType = model.DishType,

                    IngredientItems = model.IngredientItems
                        .Select(i => new DishIngredientRequestData
                        {
                            Name = i.Name,
                            Quantity = i.Quantity,
                            Unit = i.Unit
                        })
                        .ToList()
                };


            try
            {
                DishServiceStatusResponseModel response =
                    await _dishService
                        .EditDishAsync(
                            id.Value,
                            request,
                            userId);


                if (!response.Status)
                {
                    bool dishNotFound =
                        response.ErrorMessages
                            .Any(error =>
                                error ==
                                "Dish not found.");


                    if (dishNotFound)
                    {
                        return NotFound(
                            ErrorMessages.DishNotFound);
                    }


                    foreach (string? error
                             in response.ErrorMessages)
                    {
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            ModelState.AddModelError(
                                string.Empty,
                                error);
                        }
                    }


                    model.DishTypes =
                        LoadDishTypes();


                    return View(model);
                }


                TempData["Success"] =
                    UpdateDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequestTryAgain);


                TempData["Error"] =
                    UpdateDishErrorMessage;
            }


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(  [FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequestTryAgain);
            }


            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            GetDishByIdResponseModel? response =
                await _dishService
                    .GetDishForDeleteAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound(
                    ErrorMessages.DishNotFound);
            }


            DeleteDishViewModel model =
                new DeleteDishViewModel
                {
                    Id = response.Id,
                    Name = response.Name
                };


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] int? id,  DeleteDishViewModel model)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequestTryAgain);
            }


            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            try
            {
                DishServiceStatusResponseModel response =
                    await _dishService
                        .DeleteDishAsync(
                            id.Value,
                            userId);


                if (!response.Status)
                {
                    bool dishNotFound =
                        response.ErrorMessages
                            .Any(error =>
                                error ==
                                "Dish not found.");


                    if (dishNotFound)
                    {
                        return NotFound(
                            ErrorMessages.DishNotFound);
                    }


                    TempData["Error"] =
                        DeleteDishErrorMessage;


                    return RedirectToAction(
                        nameof(Index));
                }


                TempData["Success"] =
                    DeleteDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);


                TempData["Error"] =
                    DeleteDishErrorMessage;
            }


            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest( ErrorMessages.InvalidRequest);
            }


            string userId =  User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            GetDishDetailsResponseModel? response =
                await _dishService
                    .GetDishDetailsAsync(
                        id.Value,
                        userId);


            if (response == null)
            {
                return NotFound( ErrorMessages.DishNotFound);
            }


            DetailsDishViewModel model =
                new DetailsDishViewModel
                {
                    Id = response.Id,
                    Name = response.Name,
                    Description = response.Description,
                    Ingredients = response.Ingredients,
                    Instructions = response.Instructions,
                    Calories = response.Calories,
                    PreparationTime = response.PreparationTime,
                    Portions = response.Portions,
                    ImageUrl = response.ImageUrl,
                    DishType = response.DishType,
                    IngredientItems =  response.IngredientItems
                               .Select(i =>
                                   new DishIngredientInputViewModel
                                   {
                                       Name =
                                           i.Name,
                          
                                       Quantity =
                                           i.Quantity,
                          
                                       Unit =
                                           i.Unit
                                   })
                               .ToList(),
                };


            return View(model);
        }


        private static string BuildIngredientsText(
            IEnumerable<DishIngredientInputViewModel> ingredientItems)
        {
            return string.Join(
                ", ",
                ingredientItems
                    .Where(i =>
                        !string.IsNullOrWhiteSpace(
                            i.Name))
                    .Select(i =>
                        i.Name.Trim()));
        }


        private IEnumerable<DropdownViewModel> LoadDishTypes()
        {
            return _dishService
                .LoadDishTypes()
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
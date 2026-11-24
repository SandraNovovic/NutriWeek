using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Areas.Admin.ViewModels;
using NutriWeek.Services.Core.Data.Dish.Request;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dish;

using static NutriWeek.GCommon.ApplicationConstants;

namespace NutriWeek.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AdministratorRoleName)]
    public class SharedDishesController : Controller
    {
        private readonly IDishService _dishService;


        public SharedDishesController(
            IDishService dishService)
        {
            _dishService =
                dishService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<GetAllDishesResponseModel> response =
                await _dishService
                    .GetSharedDishesAsync();


            List<SharedDishViewModel> model =
                response
                    .Select(d =>
                        new SharedDishViewModel
                        {
                            Id =
                                d.Id,

                            Name =
                                d.Name,

                            DishType =
                                d.DishType,

                            Calories =
                                d.Calories,

                            PreparationTime =
                                d.PreparationTime,

                            IsDeleted =
                                d.IsDeleted
                        })
                    .ToList();


            return View(model);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AddSharedDishViewModel model =
                new AddSharedDishViewModel
                {
                    DishTypes =
                        LoadDishTypes()
                };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            AddSharedDishViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes =
                    LoadDishTypes();

                return View(model);
            }


            string adminUserId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            CreateDishRequestData request =
                new CreateDishRequestData
                {
                    Name =
                        model.Name,

                    Description =
                        model.Description,

                    Ingredients =
                        BuildIngredientsText(
                            model.IngredientItems),

                    Instructions =
                        model.Instructions,

                    Calories =
                        model.Calories,

                    PreparationTime =
                        model.PreparationTime,

                    Portions =
                        model.Portions,

                    ImageUrl =
                        model.ImageUrl,

                    DishType =
                        model.DishType,

                    IngredientItems =
                        model.IngredientItems
                            .Select(i =>
                                new DishIngredientRequestData
                                {
                                    Name =
                                        i.Name,

                                    Quantity =
                                        i.Quantity,

                                    Unit =
                                        i.Unit
                                })
                            .ToList()
                };


            DishServiceStatusResponseModel response =
                await _dishService
                    .AddDishAsync(
                        request,
                        adminUserId,
                        true);


            if (!response.Status)
            {
                foreach (string? error
                         in response.ErrorMessages)
                {
                    if (!string.IsNullOrWhiteSpace(
                            error))
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
                "Shared dish was added successfully.";


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id)
        {
            DishServiceStatusResponseModel response =
                await _dishService
                    .SetSharedDishDeletedStatusAsync(
                        id,
                        true);


            if (!response.Status)
            {
                TempData["Error"] =
                    response.ErrorMessages
                        .FirstOrDefault() ??
                    "The shared dish could not be deleted.";
            }
            else
            {
                TempData["Success"] =
                    "Shared dish was deleted successfully.";
            }


            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(
            int id)
        {
            DishServiceStatusResponseModel response =
                await _dishService
                    .SetSharedDishDeletedStatusAsync(
                        id,
                        false);


            if (!response.Status)
            {
                TempData["Error"] =
                    response.ErrorMessages
                        .FirstOrDefault() ??
                    "The shared dish could not be restored.";
            }
            else
            {
                TempData["Success"] =
                    "Shared dish was restored successfully.";
            }


            return RedirectToAction(
                nameof(Index));
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
                        Id =
                            x.Id,

                        Name =
                            x.Name
                    })
                .ToList();
        }
    }
}

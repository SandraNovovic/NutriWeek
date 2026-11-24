using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Areas.Admin.ViewModels;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Services.Core.Interfaces;

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
    }
}

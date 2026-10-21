using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;
using NutriWeek.Data.Models.Enums;
using NutriWeek.GCommon;
using NutriWeek.Services.Core.Interfaces;
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


        public DishesController(IDishService dishService,ILogger<DishesController> logger)
        {
            _dishService = dishService;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            IEnumerable<DishIndexViewModel> dishes =
                await _dishService.GetAllAsync(
                    userId);

            return View(dishes);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AddDishViewModel DishTypes =
                new AddDishViewModel
                {
                    DishTypes =
                        _dishService.LoadDishTypes()
                };

            return View(DishTypes);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddDishViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes =
                    _dishService.LoadDishTypes();

                return View(model);
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            try
            {
                await _dishService
                    .AddDishAsync(
                        model,
                        userId);

                TempData["SuccessMessage"] =
                    AddDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    ErrorMessages.InvalidRequest);

                model.DishTypes =
                    _dishService.LoadDishTypes();

                TempData["ErrorMessage"] =
                    AddDishErrorMessage;

                return View(model);
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit( [FromRoute] int? id)
        {
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages.InvalidRequest);
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            AddDishViewModel? dish =
                await _dishService
                    .GetDishForEditAsync(
                        id.Value,
                        userId);

            if (dish == null)
            {
                return NotFound(
                    ErrorMessages.DishNotFound);
            }

            return View(dish);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int? id, AddDishViewModel dish)
        {
            if (!ModelState.IsValid)
            {
                dish.DishTypes =
                    _dishService.LoadDishTypes();

                return View(dish);
            }

            bool dishTypeExists =
                _dishService.IsDishTypeValid(
                    dish.DishType);

            if (!dishTypeExists)
            {
                ModelState.AddModelError(
                    nameof(dish.DishType),
                    ErrorMessages.InvalidDishType);

                dish.DishTypes =
                    _dishService.LoadDishTypes();

                return View(dish);
            }

            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest();
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            try
            {
                bool edited =
                    await _dishService
                        .EditDishAsync(
                            id.Value,
                            dish,
                            userId);

                if (!edited)
                {
                    return NotFound();
                }

                TempData["Success"] =
                    UpdateDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    ex,
                    ErrorMessages
                        .InvalidRequestTryAgain);

                TempData["Error"] =
                    UpdateDishErrorMessage;
            }

            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int? id)
        {
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages
                        .InvalidRequestTryAgain);
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            DeleteDishViewModel? deletingDish =
                await _dishService
                    .GetDishForDeleteAsync(
                        id.Value,
                        userId);

            if (deletingDish == null)
            {
                return NotFound();
            }

            return View(deletingDish);
        }


        [HttpPost]
        public async Task<IActionResult> Delete( [FromRoute] int? id,DeleteDishViewModel dish)
        {
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages
                        .InvalidRequestTryAgain);
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            try
            {
                bool deleted =
                    await _dishService
                        .DeleteDishAsync(
                            id.Value,
                            userId);

                if (!deleted)
                {
                    return NotFound();
                }

                TempData["Success"] =
                    DeleteDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
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
            if (!id.HasValue ||
                id.Value <= 0)
            {
                return BadRequest(
                    ErrorMessages.InvalidRequest);
            }

            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            DetailsDishViewModel? dish =
                await _dishService
                    .GetDishDetailsAsync(
                        id.Value,
                        userId);

            if (dish == null)
            {
                return NotFound(
                    ErrorMessages.DishNotFound);
            }

            return View(dish);
        }
    }
}

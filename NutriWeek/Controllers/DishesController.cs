using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dishes;
using System.Security.Claims;
using static NutriWeek.GCommon.TempDateMessages;
using static NutriWeek.GCommon.ErrorMessages;
using NutriWeek.GCommon;

namespace NutriWeek.Web.Controllers
{
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
        public async Task<IActionResult> Index()
        {

            IEnumerable<DishIndexViewModel> dishes =
                await _dishService.GetAllAsync();

            return View(dishes);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AddDishViewModel DishTypes =
                new AddDishViewModel
                {
                    DishTypes =_dishService.LoadDishTypes()
                };

            return View(DishTypes);
        }


        [HttpPost]
        public async Task<IActionResult> Add( AddDishViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes =
                    _dishService.LoadDishTypes();

                return View(model);
            }

            try
            {
                await _dishService.AddDishAsync(model);

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
        public async Task<IActionResult> Edit (int? id)
        {

        }

        [HttpPost]
        public async Task<IActionResult>Edit(int? id)
        {

        }

    }
}

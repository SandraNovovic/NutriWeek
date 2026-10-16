using Microsoft.AspNetCore.Mvc;
using NutriWeek.Models;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.Dishes;
using System.Diagnostics;

namespace NutriWeek.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDishService _dishService;

        public HomeController(ILogger<HomeController> logger, IDishService dishService)
        {
            _logger = logger;
            _dishService = dishService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<DishIndexViewModel> dishes =
                await _dishService.GetAllAsync();

            return View(dishes);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

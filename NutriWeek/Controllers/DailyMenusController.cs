using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.DailyMenus;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    public class DailyMenusController : Controller
    {
        private readonly IDailyMenuService
            _dailyMenuService;


        public DailyMenusController(IDailyMenuService dailyMenuService)
        {
            _dailyMenuService =
                dailyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            List<DailyMenuIndexViewModel> dailyMenus =
                await _dailyMenuService.GetAllAsync();


            return View(dailyMenus);
        }
    }
}

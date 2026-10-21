using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.DailyMenus;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class DailyMenusController : Controller
    {
        private readonly IDailyMenuService _dailyMenuService;


        public DailyMenusController( IDailyMenuService dailyMenuService)
        {
            _dailyMenuService =dailyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;

            List<DailyMenuIndexViewModel> dailyMenus =
                await _dailyMenuService
                    .GetAllAsync( userId);


            return View(dailyMenus);
        }
    }
}

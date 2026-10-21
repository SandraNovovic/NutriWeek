using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.ViewModels.Home;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IWeeklyMenuService  _weeklyMenuService;


        public HomeController( IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService = weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            WeeklyMenuIndexViewModel? currentWeek =
                await _weeklyMenuService
                    .GetCurrentWeekAsync(
                        userId);


            HomeIndexViewModel model =
                new HomeIndexViewModel
                {
                    CurrentWeek =currentWeek
                };


            return View(model);
        }
    }
}

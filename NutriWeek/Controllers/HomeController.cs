using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.ViewModels.Home;
using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using NutriWeek.Web.Mappers;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IWeeklyMenuService _weeklyMenuService;


        public HomeController( IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService =weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId =
                User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            WeeklyMenuResponseModel? response =
                await _weeklyMenuService
                    .GetCurrentWeekAsync(
                        userId);


            WeeklyMenuIndexViewModel? currentWeek =
                response == null
                    ? null
                    : MenuViewModelMapper.MapWeeklyMenu(response);


            HomeIndexViewModel model =
                new HomeIndexViewModel
                {
                    CurrentWeek =
                        currentWeek
                };


            return View(model);
        }

    }
}
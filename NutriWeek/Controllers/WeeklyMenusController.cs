using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;

using NutriWeek.Web.Mappers;
using System.Globalization;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class WeeklyMenusController : Controller
    {
        private readonly IWeeklyMenuService _weeklyMenuService;


        public WeeklyMenusController( IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService = weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index( int? year, DateTime? weekStart)
        {
            int selectedYear;


            if (year.HasValue)
            {
                selectedYear =
                    year.Value;
            }
            else if (weekStart.HasValue)
            {
                selectedYear =
                    ISOWeek.GetYear(
                        weekStart.Value);
            }
            else
            {
                selectedYear =
                    ISOWeek.GetYear(
                        DateTime.Today);
            }


            if (selectedYear < 1 ||
                selectedYear > 9999)
            {
                return BadRequest(
                    "Invalid year.");
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            List<WeeklyMenuResponseModel> response =
                await _weeklyMenuService
                    .GetAllAsync(
                        selectedYear,
                        userId);


            List<WeeklyMenuIndexViewModel> weeklyMenus =
                response
                    .Select(MenuViewModelMapper.MapWeeklyMenu)
                    .ToList();


            ViewBag.SelectedYear =
                selectedYear;


            return View(weeklyMenus);
        }

    }
}
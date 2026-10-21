using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using System.Globalization;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class WeeklyMenusController : Controller
    {
        private readonly IWeeklyMenuService _weeklyMenuService;


        public WeeklyMenusController(IWeeklyMenuService weeklyMenuService)
        {
            _weeklyMenuService = weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int? year,DateTime? weekStart)
        {
            int selectedYear;


            if (year.HasValue)
            {
                selectedYear =year.Value;
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


            string userId =User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            List<WeeklyMenuIndexViewModel> weeklyMenus =
                await _weeklyMenuService
                    .GetAllAsync(selectedYear, userId);


            ViewBag.SelectedYear = selectedYear;


            return View(weeklyMenus);
        }
    }
}
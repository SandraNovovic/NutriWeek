using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels.DailyMenus;

using NutriWeek.Web.Mappers;
using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class DailyMenusController : Controller
    {
        private readonly IDailyMenuService _dailyMenuService;


        public DailyMenusController( IDailyMenuService dailyMenuService)
        {
            _dailyMenuService = dailyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            List<GetAllDailyMenusResponseModel> response =await _dailyMenuService .GetAllAsync(userId);

            List<DailyMenuIndexViewModel> dailyMenus = response
                    .Select(MenuViewModelMapper.MapDailyMenu)
                    .ToList();

            return View(dailyMenus);
        }

    }
}
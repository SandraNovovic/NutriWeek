using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;

namespace NutriWeek.Controllers
{
    public class WeeklyMenusController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        public WeeklyMenusController(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<WeeklyMenu> weeklyMenus = _dbContext.WeeklyMenus.Include(w=> w.DailyMenus).ThenInclude(d=>d.Meals).ThenInclude(m=>m.Dish)
                .OrderBy(w=>w.WeekStartDate).ThenBy(w=>w.DailyMenus.Count).ThenBy(w => w.Id).ToList();
            return View(weeklyMenus);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

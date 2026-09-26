using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;

namespace NutriWeek.Controllers
{
    public class DailyMenusController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        public DailyMenusController(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<DailyMenu> dailyMenus = _dbContext.DailyMenus.Include(d=>d.Meals).ThenInclude(m=>m.Dish)
                .OrderBy(d=>d.Meals.Count).ThenBy(d => d.Date).ToList();
            return View(dailyMenus);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

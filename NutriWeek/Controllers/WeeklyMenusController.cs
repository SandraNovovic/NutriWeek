using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;

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
            return View();
        }
    }
}

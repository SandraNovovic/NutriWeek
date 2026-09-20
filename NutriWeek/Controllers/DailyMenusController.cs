using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;

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
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;
using NutriWeek.Data.Models;

namespace NutriWeek.Controllers
{
    public class MealsController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        public MealsController(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<Meal> meals = _dbContext.Meals.OrderBy(m=>m.Id).ToList();
            return View(meals);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

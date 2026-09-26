using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

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
            List<Meal> meals = _dbContext.Meals.Include(m=>m.Dish).Include(m=>m.DailyMenu)
                .OrderBy(m=>m.DailyMenu.Date).ThenBy(m=>m.MealType).ToList();
            return View(meals);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

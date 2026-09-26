using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;
using NutriWeek.Data.Models;

namespace NutriWeek.Controllers
{
    public class DishesController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        public DishesController(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            List<Dish> dishes = _dbContext.Dishes.OrderBy(d=>d.Calories).ThenBy(d=>d.Id).ToList();
            return View(dishes);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

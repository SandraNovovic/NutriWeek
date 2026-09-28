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

        [HttpGet]
        public IActionResult Index()
        {
            List<Dish> dishes = _dbContext.Dishes.OrderBy(d=>d.Calories).ThenBy(d=>d.Id).ToList();
            return View(dishes);
        }

        [HttpGet]
        public IActionResult AddDish()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDish(Dish dish) 
        {
            try
            {
                _dbContext.Dishes.Add(dish);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return View(dish);
            }

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

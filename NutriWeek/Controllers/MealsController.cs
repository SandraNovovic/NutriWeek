using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.ViewModels.Meals;

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
            List<MealIndexViewModel> meals = _dbContext
                .Meals
                .Include(m=>m.Dish)
                .Include(m=>m.DailyMenu)
                .OrderBy(m=>m.DailyMenu.Date).ThenBy(m=>m.MealType)
                .Select(m=>new MealIndexViewModel
                {
                    Id = m.Id,
                    Date = m.DailyMenu.Date,
                    DishName = m.Dish.Name,
                    Calories = m.Dish.Calories,
                    Preparation = m.Dish.PreparationTime,
                    MealType = m.MealType
                })
                .ToList();

            return View(meals);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.ViewModels.DailyMenus;
using NutriWeek.ViewModels.WeeklyMenus;

namespace NutriWeek.Controllers
{
    public class WeeklyMenusController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        public WeeklyMenusController(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<WeeklyMenuIndexViewModel> weeklyMenus = _dbContext.WeeklyMenus
                .OrderByDescending(w => w.WeekStartDate)
                .Select(w => new WeeklyMenuIndexViewModel
                {
                    Id = w.Id,
                    StartDate = w.WeekStartDate,
                    TotalCalories = w.DailyMenus.SelectMany(d=>d.Meals).Sum(m=>m.Dish.Calories),
                    TotalPreparationTime = w.DailyMenus.SelectMany(d => d.Meals).Sum(m => m.Dish.PreparationTime),
                    TotalMeals = w.DailyMenus.Sum(d => d.Meals.Count),
                    TotalPortions = w.DailyMenus.SelectMany(d => d.Meals).Sum(m => m.Dish.Portions),
                    DailyMenus = w.DailyMenus.OrderBy(d=>d.Date)
                    .Select(d => new DailyMenuIndexViewModel
                    {
                        Date = d.Date,
                        Calories = d.Meals.Sum(m => m.Dish.Calories),
                        PreparationTime = d.Meals.Sum(m => m.Dish.PreparationTime),
                        TotalMeals = d.Meals.Count,
                        Portions = d.Meals.Sum(m => m.Dish.Portions)
                    }).ToList()
                })
                .ToList();

            return View(weeklyMenus);
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

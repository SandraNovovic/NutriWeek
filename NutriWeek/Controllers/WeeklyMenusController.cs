using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
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
                    TotalCalories = w.DailyMenus.SelectMany(d => d.Meals).Sum(m => m.Dish.Calories),
                    TotalPreparationTime = w.DailyMenus.SelectMany(d => d.Meals).Sum(m => m.Dish.PreparationTime),
                    TotalMeals = w.DailyMenus.SelectMany(d => d.Meals).Count(),
                    TotalPortions = w.DailyMenus.SelectMany(d => d.Meals).Sum(m => m.Dish.Portions),
                    DailyMenus = w.DailyMenus.OrderBy(d => d.Date)
                    .Select(d => new DailyMenuIndexViewModel
                    {
                        Date = d.Date,
                        Calories = d.Meals.Sum(m => m.Dish.Calories),
                        PreparationTime = d.Meals.Sum(m => m.Dish.PreparationTime),
                        TotalMeals = d.Meals.Count,
                        Portions = d.Meals.Sum(m => m.Dish.Portions),
                        BreakfastMeals = d.Meals
                        .Where(m => m.MealType == MealType.Breakfast)
                        .Select(m => new DailyMenuMealViewModel
                        {
                            MealId = m.Id,
                            DishId = m.DishId,
                            DishName = m.Dish.Name,
                            Description = m.Dish.Description,
                            DishType = m.Dish.DishType.ToString(),
                            MealType = m.MealType.ToString(),
                            Calories = m.Dish.Calories,
                            PreparationTime = m.Dish.PreparationTime,
                            Portions = m.Dish.Portions,
                            ImageUrl = m.Dish.ImageUrl
                        })
                        .ToList(),

                        LunchMeals = d.Meals
                        .Where(m => m.MealType == MealType.Lunch)
                        .Select(m => new DailyMenuMealViewModel
                        {
                            MealId = m.Id,
                            DishId = m.DishId,
                            DishName = m.Dish.Name,
                            Description = m.Dish.Description,
                            DishType = m.Dish.DishType.ToString(),
                            MealType = m.MealType.ToString(),
                            Calories = m.Dish.Calories,
                            PreparationTime = m.Dish.PreparationTime,
                            Portions = m.Dish.Portions,
                            ImageUrl = m.Dish.ImageUrl
                        })
                        .ToList(),

                        DinnerMeals = d.Meals
                        .Where(m => m.MealType == MealType.Dinner)
                        .Select(m => new DailyMenuMealViewModel
                        {
                            MealId = m.Id,
                            DishId = m.DishId,
                            DishName = m.Dish.Name,
                            Description = m.Dish.Description,
                            DishType = m.Dish.DishType.ToString(),
                            MealType = m.MealType.ToString(),
                            Calories = m.Dish.Calories,
                            PreparationTime = m.Dish.PreparationTime,
                            Portions = m.Dish.Portions,
                            ImageUrl = m.Dish.ImageUrl
                        })
                        .ToList()
                    })
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

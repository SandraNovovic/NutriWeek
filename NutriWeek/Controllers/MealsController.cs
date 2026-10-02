using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.ViewModels;
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

        [HttpGet]
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

        [HttpGet]
        public IActionResult AddMeal()
        {
            IEnumerable<DropdownViewModel> dishes = _dbContext.Dishes
                .Select(d => new DropdownViewModel
                {
                    Id = d.Id,
                    Name = d.Name
                })
                .ToList();


            AddMealViewModel model = new AddMealViewModel
            {
                Dishes = dishes,
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMeal(AddMealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Dishes = _dbContext.Dishes
                    .Select(d => new DropdownViewModel
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToList();

                return View(model);
            }

            try
            {
                DateTime selectedDate = model.Date!.Value.Date;

                int difference =
                    ((int)selectedDate.DayOfWeek -
                     (int)DayOfWeek.Monday + 7) % 7;

                DateTime weekStartDate =
                    selectedDate.AddDays(-difference);


                WeeklyMenu? weeklyMenu = _dbContext.WeeklyMenus
                    .FirstOrDefault(w =>
                        w.WeekStartDate.Date == weekStartDate);


                if (weeklyMenu == null)
                {
                    weeklyMenu = new WeeklyMenu
                    {
                        WeekStartDate = weekStartDate,
                        Title = $"Week of {weekStartDate:dd.MM.yyyy}"
                    };

                    _dbContext.WeeklyMenus.Add(weeklyMenu);
                    _dbContext.SaveChanges();
                }


                DailyMenu? dailyMenu = _dbContext.DailyMenus
                    .FirstOrDefault(d =>
                        d.Date.Date == selectedDate);


                if (dailyMenu == null)
                {
                    dailyMenu = new DailyMenu
                    {
                        Date = selectedDate,
                        WeeklyMenuId = weeklyMenu.Id
                    };

                    _dbContext.DailyMenus.Add(dailyMenu);
                    _dbContext.SaveChanges();
                }


                Meal meal = new Meal
                {
                    DishId = model.DishId,
                    MealType = model.MealType,
                    DailyMenuId = dailyMenu.Id
                };


                _dbContext.Meals.Add(meal);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                model.Dishes = _dbContext.Dishes
                    .Select(d => new DropdownViewModel
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToList();

                return View(model);
            }


            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}

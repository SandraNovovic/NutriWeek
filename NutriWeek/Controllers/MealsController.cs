using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.ViewModels;
using NutriWeek.ViewModels.Dishes;
using NutriWeek.ViewModels.Meals;
using static NutriWeek.Data.TempDateMessages;
using static NutriWeek.Data.AppConstants;
using static NutriWeek.Data.TempDateMessages;
namespace NutriWeek.Controllers
{
    public class MealsController : Controller
    {
        private readonly NutriWeekDbContext _dbContext;
        private readonly ILogger<MealsController> _logger;
        public MealsController(NutriWeekDbContext dbContext, ILogger<MealsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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

            AddMealViewModel dishes = new AddMealViewModel
            {
                Dishes = LoadDishes(),
            };

            return View(dishes);
        }


        [HttpPost]
        public IActionResult AddMeal(AddMealViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                model.Dishes = LoadDishes();

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
                    TempData["SuccessMessage"] = AddDailyMenuSuccessMessage;
                }


                Meal meal = new Meal
                {
                    DishId = model.DishId,
                    MealType = model.MealType,
                    DailyMenuId = dailyMenu.Id
                };


                _dbContext.Meals.Add(meal);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = AddMealSuccessMessage;
            }
            catch (Exception ex)
            {
                model.Dishes = LoadDishes();

                TempData["ErrorMessage"] = AddMealErrorMessage;
                return View(model);
            }


            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit([FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(
                    "There was an error with your request!"
                );
            }

            AddMealViewModel? meal = _dbContext.Meals
                .Where(m => m.Id == id.Value)
                .Select(m => new AddMealViewModel
                {
                    DishId = m.DishId,
                    MealType = m.MealType,
                    Date = m.DailyMenu.Date
                })
                .SingleOrDefault();

            if (meal == null)
            {
                return NotFound("Meal not found.");
            }

            meal.Dishes = LoadDishes();

            return View(meal);
        }

        [HttpPost]
        public IActionResult Edit( [FromRoute] int? id,AddMealViewModel meal)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                meal.Dishes = LoadDishes();

                return View(meal);
            }

            bool mealTypeExists =
                Enum.IsDefined(
                    typeof(MealType),
                    meal.MealType
                );

            if (!mealTypeExists)
            {
                ModelState.AddModelError(
                    nameof(meal.MealType),
                    "Selected meal type does not exist."
                );

                meal.Dishes = LoadDishes();

                return View(meal);
            }

            bool dishExists =
                _dbContext.Dishes
                    .Any(d => d.Id == meal.DishId);

            if (!dishExists)
            {
                ModelState.AddModelError(
                    nameof(meal.DishId),
                    "Selected dish does not exist."
                );

                meal.Dishes = LoadDishes();

                return View(meal);
            }

            Meal? mealToEdit =
                _dbContext.Meals
                    .Find(id.Value);

            if (mealToEdit == null)
            {
                return NotFound();
            }

            try
            {
                DateTime selectedDate =
                    meal.Date!.Value.Date;


                int difference =
                    ((int)selectedDate.DayOfWeek -
                     (int)DayOfWeek.Monday + 7) % 7;


                DateTime weekStartDate =
                    selectedDate.AddDays(-difference);


                WeeklyMenu? weeklyMenu =
                    _dbContext.WeeklyMenus
                        .FirstOrDefault(w =>
                            w.WeekStartDate.Date ==
                            weekStartDate);


                if (weeklyMenu == null)
                {
                    weeklyMenu = new WeeklyMenu
                    {
                        WeekStartDate = weekStartDate
                    };

                    _dbContext.WeeklyMenus.Add(
                        weeklyMenu
                    );

                    _dbContext.SaveChanges();
                }


                DailyMenu? dailyMenu =
                    _dbContext.DailyMenus
                        .FirstOrDefault(d =>
                            d.Date.Date ==
                            selectedDate);


                if (dailyMenu == null)
                {
                    dailyMenu = new DailyMenu
                    {
                        Date = selectedDate,
                        WeeklyMenuId = weeklyMenu.Id
                    };

                    _dbContext.DailyMenus.Add(
                        dailyMenu
                    );

                    _dbContext.SaveChanges();
                }


                mealToEdit.DishId =
                    meal.DishId;

                mealToEdit.MealType =
                    meal.MealType;

                mealToEdit.DailyMenuId =
                    dailyMenu.Id;


                _dbContext.SaveChanges();


                TempData["Success"] = UpdateMealSuccessMessage;
            }
            catch (Exception)
            {
                TempData["Error"] = UpdateMealErrorMessage;

                meal.Dishes = LoadDishes();

                return View(meal);
            }


            return RedirectToAction(
                nameof(Index)
            );
        }

        [HttpGet]
        public IActionResult Delete([FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(
                    "There was an error with your request! Try again!"
                );
            }

            DeleteMealViewModel? deletingMeal = _dbContext.Meals
                .Where(m => m.Id == id.Value)
                .Select(m => new DeleteMealViewModel
                {
                    Id = m.Id,
                    DishName = m.Dish.Name,
                    Date = m.DailyMenu.Date,
                    MealType = m.MealType.ToString(),
                    ImageUrl = m.Dish.ImageUrl
                })
                .SingleOrDefault();

            if (deletingMeal == null)
            {
                return NotFound("Meal not found.");
            }

            return View(deletingMeal);
        }

        [HttpPost]
        public IActionResult Delete([FromRoute] int? id,DeleteMealViewModel meal)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(
                    "There was an error with your request! Try again!"
                );
            }

            Meal? mealToDelete =
                _dbContext.Meals.Find(id.Value);

            if (mealToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _dbContext.Meals.Remove(mealToDelete);

                _dbContext.SaveChanges();

                TempData["Success"] =DeleteMealSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while deleting the meal."
                );

                TempData["Error"] =DeleteMealErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(
            [FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(
                    "There was an error with your request!"
                );
            }


            MealDetailsViewModel? meal =
                _dbContext.Meals
                    .Where(m =>
                        m.Id == id.Value)
                    .Select(m =>
                        new MealDetailsViewModel
                        {
                            Id = m.Id,

                            Date =
                                m.DailyMenu.Date,

                            DishName =
                                m.Dish.Name,

                            Description =
                                m.Dish.Description,

                            DishType =
                                m.Dish.DishType
                                    .ToString(),

                            MealType =
                                m.MealType
                                    .ToString(),

                            Calories =
                                m.Dish.Calories,

                            PreparationTime =
                                m.Dish.PreparationTime,

                            Portions =
                                m.Dish.Portions,

                            ImageUrl =
                                m.Dish.ImageUrl
                        })
                    .SingleOrDefault();


            if (meal == null)
            {
                return NotFound(
                    "Meal not found."
                );
            }


            return View(meal);
        }

        private IEnumerable<DropdownViewModel> LoadDishes()
        {
            return  _dbContext.Dishes
               .Select(d => new DropdownViewModel
               {
                   Id = d.Id,
                   Name = d.Name
               })
               .ToList();
        }
    }
}

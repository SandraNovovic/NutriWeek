using Microsoft.AspNetCore.Mvc;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.ViewModels;
using NutriWeek.ViewModels.Dishes;
using static NutriWeek.Data.TempDateMessages;
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
            IEnumerable<DishIndexViewModel> dishes = _dbContext.Dishes
                .OrderBy(d=>d.Calories)
                .ThenBy(d=>d.Id)
                .Select(d => new DishIndexViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                Calories = d.Calories,
                PreparationTime = d.PreparationTime,
                Portions = d.Portions,
                ImageUrl = d.ImageUrl,
                DishType = d.DishType
            })
                .ToList();

            return View(dishes);
        }

        [HttpGet]
        public IActionResult AddDish()
        {


            AddDishViewModel DishTypes = new AddDishViewModel
            {
                DishTypes = LoadDishTypes()
            };
            return View(DishTypes);
        }

        [HttpPost]
        public IActionResult AddDish(AddDishViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes = LoadDishTypes();
                return View(model);
            }


            try
            {
                Dish dish = new Dish
                {
                    Name = model.Name,
                    Description = model.Description,
                    Ingredients = model.Ingredients,
                    Instructions = model.Instructions,
                    Calories = model.Calories,
                    PreparationTime = model.PreparationTime,
                    Portions = model.Portions,
                    ImageUrl = model.ImageUrl,
                    DishType = model.DishType
                };
                _dbContext.Dishes.Add(dish);
                _dbContext.SaveChanges();
                TempData["SuccessMessage"] = AddDishSuccessMessage;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = AddDishErrorMessage;
                return View(model);
            }

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            return View();
        }

        private IEnumerable<DropdownViewModel> LoadDishTypes()
        {
            return Enum.GetValues<DishType>()
                .Select(dt => new DropdownViewModel
                {
                    Id = (int)dt,
                    Name = dt.ToString()
                })
                .ToList();
        }
    }
}

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
        private readonly ILogger<DishesController> _logger;
        public DishesController(NutriWeekDbContext dbContext,ILogger<DishesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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

        [HttpGet]
        public IActionResult Edit([FromRoute] int? id)
        {     
            if(!id.HasValue || id.Value <= 0)
            {
                return BadRequest(ErrorMessages.InvalidRequest); 
            }

            AddDishViewModel? dish = _dbContext.Dishes
                .Where(d=> d.Id == id.Value)
                .Select(d=>new AddDishViewModel()
                {
                    Name = d.Name,
                    Description = d.Description,
                    Ingredients= d.Ingredients,
                    Instructions = d.Instructions,
                    Calories = d.Calories,
                    PreparationTime = d.PreparationTime,
                    Portions = d.Portions,
                    ImageUrl = d.ImageUrl,
                    DishType=d.DishType
                })
                .SingleOrDefault();

            if(dish == null)
            {
                return NotFound(ErrorMessages.DishNotFound);
            }

            dish.DishTypes = LoadDishTypes();

            return View(dish);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute] int? id,AddDishViewModel dish)
        {
            if(!ModelState.IsValid)
            {
                dish.DishTypes = LoadDishTypes();
                return View(dish);
            }

            bool dishTypeExists= Enum.IsDefined(typeof(DishType),dish.DishType);

            if(!dishTypeExists)
            {
                ModelState.AddModelError(nameof(dish.DishType), ErrorMessages.InvalidDishType);
                dish.DishTypes= LoadDishTypes();
                return View(dish);
            }

            if(!id.HasValue || id.Value<=0)
            {
                return BadRequest();

            }

            Dish? dishToEdit=_dbContext.Dishes.Find(id);

            if(dishToEdit == null)
            {
                return NotFound();
            }

            try
            {
                dishToEdit.Name = dish.Name;
                dishToEdit.Description = dish.Description;
                dishToEdit.Ingredients = dish.Ingredients;
                dishToEdit.Instructions = dish.Instructions;
                dishToEdit.Calories = dish.Calories;
                dishToEdit.PreparationTime = dish.PreparationTime;
                dishToEdit.Portions = dish.Portions;
                dishToEdit.ImageUrl = dish.ImageUrl;
                dishToEdit.DishType = dish.DishType;

                _dbContext.SaveChanges();
                TempData["Success"] = UpdateDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.InvalidRequestTryAgain);
                TempData["Error"] = UpdateDishErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete([FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest("There was an error with your request! Try again!");
            }

            DeleteDishViewModel? deletingDish = _dbContext.Dishes
                .Select(d => new DeleteDishViewModel()
                {
                    Id = d.Id,
                    Name = d.Name,
                })
                .SingleOrDefault(d => d.Id == id);

            if (deletingDish == null)
            {
                return NotFound();
            }

            return View(deletingDish);
        }

        [HttpPost]
        public IActionResult Delete([FromRoute] int? id, DeleteDishViewModel dish)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(ErrorMessages.InvalidRequestTryAgain);
            }

            Dish? dishToDelete = _dbContext.Dishes.Find(id);
            if (dishToDelete == null)
            {
                return NotFound();
            }

            try
            {

                _dbContext.Dishes.Remove(dishToDelete);
                _dbContext.SaveChanges();
                TempData["Success"] = DeleteDishSuccessMessage;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ErrorMessages.InvalidRequest);

                TempData["Error"] = DeleteDishErrorMessage;

            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details([FromRoute] int? id)
        {
            if (!id.HasValue || id.Value <= 0)
            {
                return BadRequest(
                    "There was an error with your request!"
                );
            }

            DetailsDishViewModel? dish = _dbContext.Dishes
                .Where(d => d.Id == id.Value)
                .Select(d => new DetailsDishViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Ingredients = d.Ingredients,
                    Instructions = d.Instructions,
                    Calories = d.Calories,
                    PreparationTime = d.PreparationTime,
                    Portions = d.Portions,
                    ImageUrl = d.ImageUrl,
                    DishType = d.DishType
                })
                .SingleOrDefault();

            if (dish == null)
            {
                return NotFound("Dish not found.");
            }

            return View(dish);
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

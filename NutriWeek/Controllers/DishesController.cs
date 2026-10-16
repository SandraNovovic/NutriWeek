using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dishes;

namespace NutriWeek.Web.Controllers
{
    public class DishesController : Controller
    {
        private readonly NutriWeekDbContext _dbCcontext;
        public DishesController(NutriWeekDbContext dbContext)
        {
                this._dbCcontext= dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<DishIndexViewModel> dishes =
                await _dbCcontext.Dishes
                    .AsNoTracking()
                    .Select(d =>
                        new DishIndexViewModel
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Description = d.Description,
                            Calories = d.Calories,
                            PreparationTime = d.PreparationTime,
                            Portions = d.Portions,
                            ImageUrl = d.ImageUrl,
                            DishType = d.DishType,
                        })
                    .ToListAsync();


            return View(dishes);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AddDishViewModel model = new AddDishViewModel
            {
                DishTypes = LoadDishTypes()
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddDishViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DishTypes = LoadDishTypes();

                return View(model);
            }

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

            await _dbCcontext.Dishes.AddAsync(dish);

            await _dbCcontext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<DropdownViewModel> LoadDishTypes()
        {
            IEnumerable<DropdownViewModel> types = Enum.GetValues<DishType>()
                .Select(t => new DropdownViewModel
                {
                    Id = (int)t,
                    Name = t.ToString()
                })
                .ToList();

            return types;
        }
    }
}

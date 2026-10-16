using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
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
    }
}

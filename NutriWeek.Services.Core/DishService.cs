using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dishes;

namespace NutriWeek.Services.Core.Services
{
    public class DishService : IDishService
    {
        private readonly NutriWeekDbContext _dbContext;

        public DishService(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IEnumerable<DishIndexViewModel>> GetAllAsync()
        {
            List<DishIndexViewModel> dishes =
                await _dbContext.Dishes
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
                            DishType = d.DishType
                        })
                    .ToListAsync();

            return dishes;
        }


        public IEnumerable<DropdownViewModel> LoadDishTypes()
        {
            return Enum.GetValues<DishType>()
                .Select(dt => new DropdownViewModel
                {
                    Id = (int)dt,
                    Name = dt.ToString()
                })
                .ToList();
        }


        public async Task AddDishAsync(AddDishViewModel model)
        {
            Dish dish =
                new Dish
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

            await _dbContext.Dishes.AddAsync(dish);

            await _dbContext.SaveChangesAsync();
        }

    }
}

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

        public async Task<AddDishViewModel?> GetDishForEditAsync(int id)
        {
            AddDishViewModel? dish =
                await _dbContext.Dishes
                    .AsNoTracking()
                    .Select(d =>
                        new AddDishViewModel
                        {
                            Name = d.Name,
                            Description = d.Description,
                            Ingredients = d.Ingredients,
                            Instructions = d.Instructions,
                            Calories = d.Calories,
                            PreparationTime =
                                d.PreparationTime,
                            Portions = d.Portions,
                            ImageUrl = d.ImageUrl,
                            DishType = d.DishType
                        })
                    .SingleOrDefaultAsync();

            if (dish != null)
            {
                dish.DishTypes =
                    LoadDishTypes();
            }

            return dish;
        }


        public bool IsDishTypeValid(DishType dishType)
        {
            return Enum.IsDefined(
                typeof(DishType),
                dishType);
        }


        public async Task<bool> EditDishAsync(int id,AddDishViewModel dish)
        {
            Dish? dishToEdit =
                await _dbContext.Dishes
                    .FirstOrDefaultAsync(d =>d.Id == id);

            if (dishToEdit == null)
            {
                return false;
            }

            dishToEdit.Name =
                dish.Name;

            dishToEdit.Description =
                dish.Description;

            dishToEdit.Ingredients =
                dish.Ingredients;

            dishToEdit.Instructions =
                dish.Instructions;

            dishToEdit.Calories =
                dish.Calories;

            dishToEdit.PreparationTime =
                dish.PreparationTime;

            dishToEdit.Portions =
                dish.Portions;

            dishToEdit.ImageUrl =
                dish.ImageUrl;

            dishToEdit.DishType =
                dish.DishType;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DeleteDishViewModel?> GetDishForDeleteAsync(int id)
        {
            DeleteDishViewModel? deletingDish =
                await _dbContext.Dishes
                    .AsNoTracking()
                    .Select(d =>
                        new DeleteDishViewModel
                        {
                            Id = d.Id,
                            Name = d.Name
                        })
                    .SingleOrDefaultAsync();

            return deletingDish;
        }


        public async Task<bool> DeleteDishAsync(int id)
        {
            Dish? dishToDelete =
                await _dbContext.Dishes
                    .FirstOrDefaultAsync(d => d.Id == id);

            if (dishToDelete == null)
            {
                return false;
            }

            _dbContext.Dishes.Remove(dishToDelete);

            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<DetailsDishViewModel?> GetDishDetailsAsync(int id)
        {
            DetailsDishViewModel? dish =
                await _dbContext.Dishes
                    .AsNoTracking()
                    .Select(d =>
                        new DetailsDishViewModel
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Description =
                                d.Description,
                            Ingredients =
                                d.Ingredients,
                            Instructions =
                                d.Instructions,
                            Calories =
                                d.Calories,
                            PreparationTime =
                                d.PreparationTime,
                            Portions =
                                d.Portions,
                            ImageUrl =
                                d.ImageUrl,
                            DishType =
                                d.DishType
                        })
                    .SingleOrDefaultAsync();

            return dish;
        }



    }
}

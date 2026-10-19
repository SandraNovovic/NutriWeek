using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Meals;
using System;

namespace NutriWeek.Services.Core.Implementations
{
    public class MealService : IMealService
    {
        private readonly NutriWeekDbContext _dbContext;

        public MealService(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<MealIndexViewModel>> GetAllAsync()
        {
            List<MealIndexViewModel> meals =
                await _dbContext.Meals
                    .AsNoTracking()
                    .OrderBy(m => m.DailyMenu.Date)
                    .ThenBy(m => m.MealType)
                    .Select(m =>
                        new MealIndexViewModel
                        {
                            Id = m.Id,
                            Date = m.DailyMenu.Date,
                            DishName = m.Dish.Name,
                            Calories = m.Dish.Calories,
                            Preparation =
                            m.Dish.PreparationTime,
                            MealType = m.MealType
                        })
                    .ToListAsync();

            return meals;
        }


        public async Task<IEnumerable<DropdownViewModel>> LoadDishesAsync()
        {
            IEnumerable<DropdownViewModel> dishes =
                await _dbContext.Dishes
                    .AsNoTracking()
                    .Select(d =>
                        new DropdownViewModel
                        {
                            Id = d.Id,
                            Name = d.Name
                        })
                    .ToListAsync();

            return dishes;
        }

        public IEnumerable<DropdownViewModel> LoadMealTypes()
        {
            return Enum.GetValues<MealType>()
                .Select(m => new DropdownViewModel
                {
                    Id = (int)m,
                    Name = m.ToString()
                })
                .ToList();

        }


        public async Task<bool> DishExistsAsync(int dishId)
        {
            return await _dbContext.Dishes.AnyAsync(d =>d.Id == dishId);
        }

        public async Task<bool> AddMealAsync(AddMealViewModel model)
        {

            DateTime selectedDate =model.Date!.Value.Date;

            DailyMenu? dailyMenu =
                await _dbContext.DailyMenus
                    .FirstOrDefaultAsync(d => d.Date.Date == selectedDate);


            bool dailyMenuCreated = false;


            if (dailyMenu == null)
            {
                dailyMenu =
                    new DailyMenu
                    {
                        Date = selectedDate
                    };

                await _dbContext.DailyMenus
                    .AddAsync(dailyMenu);

                await _dbContext
                    .SaveChangesAsync();

                dailyMenuCreated = true;
            }


            Meal meal = new Meal
                {
                    DishId = model.DishId,
                    MealType = model.MealType,
                    DailyMenuId = dailyMenu.Id
                };


            await _dbContext.Meals
                .AddAsync(meal);

            await _dbContext
                .SaveChangesAsync();


            return dailyMenuCreated;

        }
    }
}

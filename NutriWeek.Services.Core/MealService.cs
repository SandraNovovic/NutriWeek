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

        public MealService(
            NutriWeekDbContext dbContext)
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
    }
}

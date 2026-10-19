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


        public async Task<AddMealViewModel?> GetMealForEditAsync(int id)
        {
            AddMealViewModel? meal =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>m.Id == id )
                    .Select(m =>
                        new AddMealViewModel
                        {
                            DishId = m.DishId,

                            MealType =
                                m.MealType,

                            Date =
                                m.DailyMenu.Date
                        })
                    .SingleOrDefaultAsync();


            if (meal != null)
            {
                meal.Dishes =
                    await LoadDishesAsync();
            }


            return meal;
        }


        public bool IsMealTypeValid(MealType mealType)
        {
            return Enum.IsDefined(typeof(MealType),mealType);
        }

        public async Task<bool> EditMealAsync(int id,AddMealViewModel meal)
        {
            Meal? mealToEdit =
                await _dbContext.Meals
                    .FirstOrDefaultAsync(m => m.Id == id);


            if (mealToEdit == null)
            {
                return false;
            }


            DateTime selectedDate =
                meal.Date!.Value.Date;


            DailyMenu? dailyMenu = await _dbContext.DailyMenus.FirstOrDefaultAsync(d =>d.Date.Date == selectedDate);


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
            }


            mealToEdit.DishId =
                meal.DishId;


            mealToEdit.MealType =
                meal.MealType;


            mealToEdit.DailyMenuId =
                dailyMenu.Id;


            await _dbContext
                .SaveChangesAsync();


            return true;
        }

        public async Task<DeleteMealViewModel?> GetMealForDeleteAsync(int id)
        {
            DeleteMealViewModel? deletingMeal =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>m.Id == id )
                    .Select(m =>
                        new DeleteMealViewModel
                        {
                            Id =
                                m.Id,

                            DishName =
                                m.Dish.Name,

                            Date =
                                m.DailyMenu.Date,

                            MealType =
                                m.MealType.ToString(),

                            ImageUrl =
                                m.Dish.ImageUrl
                        })
                    .SingleOrDefaultAsync();


            return deletingMeal;
        }


        public async Task<bool> DeleteMealAsync(int id)
        {
            Meal? mealToDelete =
                await _dbContext.Meals
                    .FirstOrDefaultAsync(m =>m.Id == id);


            if (mealToDelete == null)
            {
                return false;
            }


            _dbContext.Meals.Remove( mealToDelete);


            await _dbContext
                .SaveChangesAsync();


            return true;
        }

    }
}

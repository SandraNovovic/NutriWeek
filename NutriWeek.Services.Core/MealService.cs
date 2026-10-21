using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Meals;

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


        public async Task<List<MealIndexViewModel>> GetAllAsync(string userId)
        {
            List<MealIndexViewModel> meals =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId)
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


        public async Task<IEnumerable<DropdownViewModel>> LoadDishesAsync( string userId)
        {
            IEnumerable<DropdownViewModel> dishes =
                await _dbContext.Dishes
                    .AsNoTracking()
                    .Where(d =>
                        d.IsShared ||
                        d.UserId == userId)
                    .Select(d =>
                        new DropdownViewModel
                        {
                            Id = d.Id,
                            Name = d.Name
                        })
                    .ToListAsync();

            return dishes;
        }


        public async Task<bool> AddMealAsync( AddMealViewModel model, string userId)
        {
            DateTime selectedDate =
                model.Date!.Value.Date;


            int difference =
                ((int)selectedDate.DayOfWeek -
                 (int)DayOfWeek.Monday + 7) % 7;


            DateTime weekStartDate =
                selectedDate.AddDays(-difference);


            WeeklyMenu? weeklyMenu =
                await _dbContext.WeeklyMenus
                    .FirstOrDefaultAsync(w =>
                        w.UserId == userId &&
                        w.WeekStartDate.Date ==
                        weekStartDate);


            if (weeklyMenu == null)
            {
                weeklyMenu =
                    new WeeklyMenu
                    {
                        WeekStartDate =
                            weekStartDate,

                        Title =
                            $"Week of {weekStartDate:dd.MM.yyyy}",

                        UserId =
                            userId
                    };

                await _dbContext.WeeklyMenus
                    .AddAsync(weeklyMenu);

                await _dbContext
                    .SaveChangesAsync();
            }


            DailyMenu? dailyMenu =
                await _dbContext.DailyMenus
                    .FirstOrDefaultAsync(d =>
                        d.WeeklyMenu.UserId == userId &&
                        d.Date.Date ==
                        selectedDate);


            bool dailyMenuCreated = false;


            if (dailyMenu == null)
            {
                dailyMenu =
                    new DailyMenu
                    {
                        Date = selectedDate,

                        WeeklyMenuId =
                            weeklyMenu.Id
                    };

                await _dbContext.DailyMenus
                    .AddAsync(dailyMenu);

                await _dbContext
                    .SaveChangesAsync();

                dailyMenuCreated = true;
            }


            Meal meal =
                new Meal
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


        public async Task<AddMealViewModel?> GetMealForEditAsync( int id, string userId)
        {
            AddMealViewModel? meal =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId)
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
                    await LoadDishesAsync(
                        userId);
            }


            return meal;
        }


        public bool IsMealTypeValid(MealType mealType)
        {
            return Enum.IsDefined(
                typeof(MealType),
                mealType);
        }


        public async Task<bool> DishExistsAsync( int dishId,string userId)
        {
            return await _dbContext.Dishes
                .AnyAsync(d =>
                    d.Id == dishId &&
                    (d.IsShared ||
                     d.UserId == userId));
        }


        public async Task<bool> EditMealAsync( int id, AddMealViewModel meal,string userId)
        {
            Meal? mealToEdit =
                await _dbContext.Meals
                    .FirstOrDefaultAsync(m =>
                        m.Id == id &&
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId);


            if (mealToEdit == null)
            {
                return false;
            }


            DateTime selectedDate =
                meal.Date!.Value.Date;


            int difference =
                ((int)selectedDate.DayOfWeek -
                 (int)DayOfWeek.Monday + 7) % 7;


            DateTime weekStartDate =
                selectedDate.AddDays(-difference);


            WeeklyMenu? weeklyMenu =
                await _dbContext.WeeklyMenus
                    .FirstOrDefaultAsync(w =>
                        w.UserId == userId &&
                        w.WeekStartDate.Date ==
                        weekStartDate);


            if (weeklyMenu == null)
            {
                weeklyMenu =
                    new WeeklyMenu
                    {
                        WeekStartDate =
                            weekStartDate,

                        Title =
                            $"Week of {weekStartDate:dd.MM.yyyy}",

                        UserId =
                            userId
                    };


                await _dbContext.WeeklyMenus
                    .AddAsync(weeklyMenu);


                await _dbContext
                    .SaveChangesAsync();
            }


            DailyMenu? dailyMenu =
                await _dbContext.DailyMenus
                    .FirstOrDefaultAsync(d =>
                        d.WeeklyMenu.UserId == userId &&
                        d.Date.Date ==
                        selectedDate);


            if (dailyMenu == null)
            {
                dailyMenu =
                    new DailyMenu
                    {
                        Date =
                            selectedDate,

                        WeeklyMenuId =
                            weeklyMenu.Id
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


        public async Task<DeleteMealViewModel?> GetMealForDeleteAsync( int id, string userId)
        {
            DeleteMealViewModel? deletingMeal =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId)
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


        public async Task<bool> DeleteMealAsync( int id, string userId)
        {
            Meal? mealToDelete =
                await _dbContext.Meals
                    .FirstOrDefaultAsync(m =>
                        m.Id == id &&
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId);


            if (mealToDelete == null)
            {
                return false;
            }


            _dbContext.Meals.Remove(
                mealToDelete);


            await _dbContext
                .SaveChangesAsync();


            return true;
        }


        public async Task<MealDetailsViewModel?> GetMealDetailsAsync(int id, string userId)
        {
            MealDetailsViewModel? meal =
                await _dbContext.Meals
                    .AsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu.WeeklyMenu.UserId ==
                        userId)
                    .Select(m =>
                        new MealDetailsViewModel
                        {
                            Id =
                                m.Id,

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
                    .SingleOrDefaultAsync();


            return meal;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.DailyMenus;

namespace NutriWeek.Services.Core.Services
{
    public class DailyMenuService : IDailyMenuService
    {
        private readonly NutriWeekDbContext _dbContext;


        public DailyMenuService(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<DailyMenuIndexViewModel>> GetAllAsync()
        {
            List<DailyMenuIndexViewModel> dailyMenus =
                await _dbContext.DailyMenus
                    .AsNoTracking()
                    .OrderBy(d => d.Date)
                    .Select(d =>
                        new DailyMenuIndexViewModel
                        {
                            Date = d.Date,

                            Calories = d.Meals
                                .Sum(m =>
                                    m.Dish.Calories),

                            PreparationTime = d.Meals
                                .Sum(m =>
                                    m.Dish.PreparationTime),

                            TotalMeals =
                                d.Meals.Count,

                            Portions = d.Meals
                                .Sum(m =>
                                    m.Dish.Portions),


                            BreakfastMeals = d.Meals.Where(m =>m.MealType ==MealType.Breakfast)
                                .Select(m =>
                                    new DailyMenuMealViewModel
                                    {
                                        MealId =
                                            m.Id,

                                        DishId =
                                            m.DishId,

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
                                            m.Dish
                                                .PreparationTime,

                                        Portions =
                                            m.Dish.Portions,

                                        ImageUrl =
                                            m.Dish.ImageUrl
                                    })
                                .ToList(),


                            LunchMeals = d.Meals
                                .Where(m =>m.MealType ==MealType.Lunch)
                                .Select(m =>
                                    new DailyMenuMealViewModel
                                    {
                                        MealId =
                                            m.Id,

                                        DishId =
                                            m.DishId,

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
                                            m.Dish
                                                .PreparationTime,

                                        Portions =
                                            m.Dish.Portions,

                                        ImageUrl =
                                            m.Dish.ImageUrl
                                    })
                                .ToList(),


                            DinnerMeals = d.Meals
                                .Where(m => m.MealType ==MealType.Dinner)
                                .Select(m =>
                                    new DailyMenuMealViewModel
                                    {
                                        MealId =
                                            m.Id,

                                        DishId =
                                            m.DishId,

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
                                            m.Dish
                                                .PreparationTime,

                                        Portions =
                                            m.Dish.Portions,

                                        ImageUrl =
                                            m.Dish.ImageUrl
                                    })
                                .ToList()
                        })
                    .ToListAsync();


            return dailyMenus;
        }
    }
}

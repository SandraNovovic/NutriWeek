using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

namespace NutriWeek.Services.Core.Services
{
    public class DailyMenuService : IDailyMenuService
    {
        private readonly IDailyMenuRepository dailyMenuRepository;


        public DailyMenuService(
            IDailyMenuRepository dailyMenuRepository)
        {
            this.dailyMenuRepository =
                dailyMenuRepository;
        }


        public async Task<List<GetAllDailyMenusResponseModel>>
            GetAllAsync(string userId)
        {
            List<GetAllDailyMenusResponseModel> dailyMenus =
                await dailyMenuRepository
                    .GetAllAsNoTracking()
                    .Where(d =>
                        d.WeeklyMenu.UserId ==
                        userId)
                    .OrderBy(d =>
                        d.Date)
                    .Select(d =>
                        new GetAllDailyMenusResponseModel
                        {
                            Id =
                                d.Id,

                            Date =
                                d.Date,

                            Calories =
                                d.Meals
                                    .Sum(m =>
                                        m.Dish.Calories),

                            PreparationTime =
                                d.Meals
                                    .Sum(m =>
                                        m.Dish.PreparationTime),

                            TotalMeals =
                                d.Meals.Count(),

                            Portions =
                                d.Meals
                                    .Sum(m =>
                                        m.Dish.Portions),


                            BreakfastMeals =
                                d.Meals
                                    .Where(m =>
                                        m.MealType ==
                                        MealType.Breakfast)
                                    .Select(m =>
                                        new DailyMenuMealResponseModel
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
                                                m.Dish.PreparationTime,

                                            Portions =
                                                m.Dish.Portions,

                                            ImageUrl =
                                                m.Dish.ImageUrl
                                        })
                                    .ToList(),


                            LunchMeals =
                                d.Meals
                                    .Where(m =>
                                        m.MealType ==
                                        MealType.Lunch)
                                    .Select(m =>
                                        new DailyMenuMealResponseModel
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
                                                m.Dish.PreparationTime,

                                            Portions =
                                                m.Dish.Portions,

                                            ImageUrl =
                                                m.Dish.ImageUrl
                                        })
                                    .ToList(),


                            DinnerMeals =
                                d.Meals
                                    .Where(m =>
                                        m.MealType ==
                                        MealType.Dinner)
                                    .Select(m =>
                                        new DailyMenuMealResponseModel
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
                                                m.Dish.PreparationTime,

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
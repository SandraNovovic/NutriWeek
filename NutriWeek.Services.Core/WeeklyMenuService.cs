using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core.Data.DailyMenu.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;
using NutriWeek.Services.Core.Interfaces;

using System.Globalization;

namespace NutriWeek.Services.Core.Services
{
    public class WeeklyMenuService : IWeeklyMenuService
    {
        private readonly IWeeklyMenuRepository weeklyMenuRepository;


        public WeeklyMenuService( IWeeklyMenuRepository weeklyMenuRepository)
        {
            this.weeklyMenuRepository = weeklyMenuRepository;
        }


        public async Task<List<WeeklyMenuResponseModel>> GetAllAsync( int year,string userId)
        {
            int weeksInYear = ISOWeek.GetWeeksInYear(year);


            DateTime firstWeekStart =
                ISOWeek.ToDateTime(
                    year,
                    1,
                    DayOfWeek.Monday);


            DateTime lastWeekStart =
                ISOWeek.ToDateTime(
                    year,
                    weeksInYear,
                    DayOfWeek.Monday);


            List<WeeklyMenu> weeklyMenusFromDb =
                await weeklyMenuRepository
                    .GetAllAsNoTracking()
                    .Include(w =>
                        w.DailyMenus)
                    .ThenInclude(d =>
                        d.Meals)
                    .ThenInclude(m =>
                        m.Dish)
                    .Where(w =>
                        w.UserId == userId &&
                        w.WeekStartDate >= firstWeekStart &&
                        w.WeekStartDate <= lastWeekStart)
                    .OrderBy(w =>
                        w.WeekStartDate)
                    .ToListAsync();


            List<WeeklyMenuResponseModel> plannedWeeks =
                weeklyMenusFromDb
                    .Select(MapWeeklyMenu)
                    .ToList();


            List<WeeklyMenuResponseModel> allWeeks =
                new List<WeeklyMenuResponseModel>();


            for (int weekNumber = 1; weekNumber <= weeksInYear; weekNumber++)
            {
                DateTime weekStart =
                    ISOWeek.ToDateTime(
                        year,
                        weekNumber,
                        DayOfWeek.Monday);


                WeeklyMenuResponseModel? existingWeek =
                    plannedWeeks
                        .FirstOrDefault(w =>
                            w.StartDate.Date ==
                            weekStart.Date);


                allWeeks.Add(
                    existingWeek ??
                    CreateEmptyWeek(weekStart));
            }


            return allWeeks;
        }


        public async Task<WeeklyMenuResponseModel?> GetCurrentWeekAsync(string userId)
        {
            DateTime today = DateTime.Today;


            int currentYear =  ISOWeek.GetYear(today);


            int currentWeekNumber = ISOWeek.GetWeekOfYear(today);


            DateTime currentWeekStart =
                ISOWeek.ToDateTime(
                    currentYear,
                    currentWeekNumber,
                    DayOfWeek.Monday);


            List<WeeklyMenuResponseModel> weeklyMenus =
                await GetAllAsync(
                    currentYear,
                    userId);


            return weeklyMenus
                .FirstOrDefault(w =>
                    w.StartDate.Date ==
                    currentWeekStart.Date);
        }


        private static WeeklyMenuResponseModel MapWeeklyMenu(WeeklyMenu weeklyMenu)
        {
            List<GetAllDailyMenusResponseModel> dailyMenus =
                weeklyMenu.DailyMenus
                    .OrderBy(d => d.Date)
                    .Select(MapDailyMenu)
                    .ToList();


            List<Meal> meals =
                weeklyMenu.DailyMenus
                    .SelectMany(d => d.Meals)
                    .ToList();


            return new WeeklyMenuResponseModel
            {
                Id =
                    weeklyMenu.Id,

                StartDate =
                    weeklyMenu.WeekStartDate,

                TotalCalories =
                    meals.Sum(m =>
                        m.Dish.Calories),

                TotalPreparationTime =
                    meals.Sum(m =>
                        m.Dish.PreparationTime),

                TotalMeals =
                    meals.Count,

                TotalPortions =
                    meals.Sum(m =>
                        m.Dish.Portions),

                DailyMenus =
                    dailyMenus
            };
        }


        private static GetAllDailyMenusResponseModel MapDailyMenu( DailyMenu dailyMenu)
        {
            return new GetAllDailyMenusResponseModel
            {
                Id =
                    dailyMenu.Id,

                Date =
                    dailyMenu.Date,

                Calories =
                    dailyMenu.Meals
                        .Sum(m =>
                            m.Dish.Calories),

                PreparationTime =
                    dailyMenu.Meals
                        .Sum(m =>
                            m.Dish.PreparationTime),

                TotalMeals =
                    dailyMenu.Meals.Count,

                Portions =
                    dailyMenu.Meals
                        .Sum(m =>
                            m.Dish.Portions),

                BreakfastMeals =
                    dailyMenu.Meals
                        .Where(m =>
                            m.MealType ==
                            MealType.Breakfast)
                        .Select(MapMeal)
                        .ToList(),

                LunchMeals =
                    dailyMenu.Meals
                        .Where(m =>
                            m.MealType ==
                            MealType.Lunch)
                        .Select(MapMeal)
                        .ToList(),

                DinnerMeals =
                    dailyMenu.Meals
                        .Where(m =>
                            m.MealType ==
                            MealType.Dinner)
                        .Select(MapMeal)
                        .ToList()
            };
        }


        private static DailyMenuMealResponseModel MapMeal( Meal meal)
        {
            return new DailyMenuMealResponseModel
            {
                MealId =
                    meal.Id,

                DishId =
                    meal.DishId,

                DishName =
                    meal.Dish.Name,

                Description =
                    meal.Dish.Description,

                DishType =
                    meal.Dish.DishType
                        .ToString(),

                MealType =
                    meal.MealType
                        .ToString(),

                Calories =
                    meal.Dish.Calories,

                PreparationTime =
                    meal.Dish.PreparationTime,

                Portions =
                    meal.Dish.Portions,

                ImageUrl =
                    meal.Dish.ImageUrl
            };
        }


        private static WeeklyMenuResponseModel CreateEmptyWeek( DateTime weekStart)
        {
            return new WeeklyMenuResponseModel
            {
                Id = 0,

                StartDate =
                    weekStart,

                TotalCalories = 0,

                TotalPreparationTime = 0,

                TotalMeals = 0,

                TotalPortions = 0,

                DailyMenus =
                    new List<GetAllDailyMenusResponseModel>()
            };
        }
    }
}
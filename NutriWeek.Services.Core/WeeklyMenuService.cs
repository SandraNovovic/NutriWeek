using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.DailyMenus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using Microsoft.EntityFrameworkCore;


namespace NutriWeek.Services.Core
{
    public class WeeklyMenuService : IWeeklyMenuService
    {
        private readonly NutriWeekDbContext _dbContext;


        public WeeklyMenuService(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<WeeklyMenuIndexViewModel>> GetAllAsync(int year)
        {
            int weeksInYear =ISOWeek.GetWeeksInYear(year);

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


            List<WeeklyMenu> weeklyMenusFromDb = await _dbContext.WeeklyMenus
                    .AsNoTracking()
                    .Include(w =>
                        w.DailyMenus)
                    .ThenInclude(d =>
                        d.Meals)
                    .ThenInclude(m =>
                        m.Dish)
                    .Where(w =>
                        w.WeekStartDate >= firstWeekStart &&
                        w.WeekStartDate <= lastWeekStart)
                    .OrderBy(w =>
                        w.WeekStartDate)
                    .ToListAsync();


            List<WeeklyMenuIndexViewModel> plannedWeeks =
                weeklyMenusFromDb
                  .Select(week =>
                        new WeeklyMenuIndexViewModel
                        {
                            Id =
                                week.Id,

                            StartDate =
                                week.WeekStartDate,


                            TotalCalories =
                                week.DailyMenus

                                    .SelectMany(dailyMenus =>
                                        dailyMenus.Meals)

                                    .Sum(meal =>
                                        meal.Dish.Calories),


                            TotalPreparationTime =
                                week.DailyMenus

                                    .SelectMany(dailyMenus =>
                                        dailyMenus.Meals)

                                    .Sum(meal =>
                                        meal.Dish.PreparationTime),


                            TotalMeals =
                                week.DailyMenus

                                    .SelectMany(dailyMenus =>
                                        dailyMenus.Meals)

                                    .Count(),


                            TotalPortions =
                                week.DailyMenus

                                    .SelectMany(dailyMenus =>
                                        dailyMenus.Meals)

                                    .Sum(meal =>
                                        meal.Dish.Portions),


                            DailyMenus =
                                week.DailyMenus

                                    .OrderBy(dailyMenu =>
                                        dailyMenu.Date)

                                    .Select(dailyMenu =>
                                        new DailyMenuIndexViewModel
                                        {
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

                                                    .Where(meal =>
                                                        meal.MealType ==
                                                        MealType.Breakfast)

                                                    .Select(meal =>
                                                        new DailyMenuMealViewModel
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
                                                                meal.Dish
                                                                    .PreparationTime,

                                                            Portions =
                                                                meal.Dish.Portions,

                                                            ImageUrl =
                                                                meal.Dish.ImageUrl
                                                        })

                                                    .ToList(),


                                            LunchMeals =
                                                dailyMenu.Meals

                                                    .Where(meal =>
                                                        meal.MealType ==
                                                        MealType.Lunch)

                                                    .Select(meal =>
                                                        new DailyMenuMealViewModel
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
                                                                meal.Dish
                                                                    .PreparationTime,

                                                            Portions =
                                                                meal.Dish.Portions,

                                                            ImageUrl =
                                                                meal.Dish.ImageUrl
                                                        })

                                                    .ToList(),


                                            DinnerMeals =
                                                dailyMenu.Meals

                                                    .Where(meal =>
                                                        meal.MealType ==
                                                        MealType.Dinner)

                                                    .Select(meal =>
                                                        new DailyMenuMealViewModel
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
                                                                meal.Dish
                                                                    .PreparationTime,

                                                            Portions =
                                                                meal.Dish.Portions,

                                                            ImageUrl =
                                                                meal.Dish.ImageUrl
                                                        })

                                                    .ToList()
                                        })

                                    .ToList()
                        })

                    .ToList();


            List<WeeklyMenuIndexViewModel> allWeeks = new List<WeeklyMenuIndexViewModel>();


            for (int weekNumber = 1; weekNumber <= weeksInYear; weekNumber++)
            {
                DateTime weekStart =
                    ISOWeek.ToDateTime(
                        year,
                        weekNumber,
                        DayOfWeek.Monday);


                WeeklyMenuIndexViewModel? existingWeek =plannedWeeks
                        .FirstOrDefault(w =>
                            w.StartDate.Date ==
                            weekStart.Date);


                if (existingWeek != null)
                {
                    allWeeks.Add(existingWeek);
                }
                else
                {
                    allWeeks.Add(
                        new WeeklyMenuIndexViewModel
                        {
                            Id = 0,

                            StartDate =weekStart,

                            TotalCalories = 0,

                            TotalPreparationTime = 0,

                            TotalMeals = 0,

                            TotalPortions = 0,

                            DailyMenus =new List<DailyMenuIndexViewModel>()
                        });
                }
            }


            return allWeeks;
        }

        public async Task<WeeklyMenuIndexViewModel?> GetCurrentWeekAsync()
        {
            DateTime today =
                DateTime.Today;

            int currentYear =
                ISOWeek.GetYear(
                    today);

            int currentWeekNumber =
                ISOWeek.GetWeekOfYear(
                    today);

            DateTime currentWeekStart =
                    ISOWeek.ToDateTime(
                        currentYear,
                        currentWeekNumber,
                        DayOfWeek.Monday);


            List<WeeklyMenuIndexViewModel> weeklyMenus =
                    await GetAllAsync(currentYear);


            WeeklyMenuIndexViewModel? currentWeek =
                    weeklyMenus
                        .FirstOrDefault(w => w.StartDate.Date ==currentWeekStart.Date);


            return currentWeek;
        }

    }
}

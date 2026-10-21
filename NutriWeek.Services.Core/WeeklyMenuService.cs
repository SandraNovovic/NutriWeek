using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using System.Globalization;

namespace NutriWeek.Services.Core.Services
{
    public class WeeklyMenuService : IWeeklyMenuService
    {
        private readonly NutriWeekDbContext _dbContext;


        public WeeklyMenuService(
            NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<WeeklyMenuIndexViewModel>> GetAllAsync(int year,string userId)
        {
            int weeksInYear =
                ISOWeek.GetWeeksInYear(year);

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
                await _dbContext.WeeklyMenus
                    .AsNoTracking()
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


            List<WeeklyMenuIndexViewModel> plannedWeeks =weeklyMenusFromDb
                    .Select(w =>
                        new WeeklyMenuIndexViewModel
                        {
                            Id = w.Id,
                            StartDate = w.WeekStartDate,
                            TotalCalories = w.DailyMenus
                                    .SelectMany(d =>d.Meals)
                                    .Sum(m =>m.Dish.Calories),
                            TotalPreparationTime =w.DailyMenus
                                    .SelectMany(d =>d.Meals).Sum(m => m.Dish.PreparationTime),
                            TotalMeals =w.DailyMenus.SelectMany(d => d.Meals).Count(),
                            TotalPortions = w.DailyMenus.SelectMany(d => d.Meals).Sum(m =>m.Dish.Portions),
                            DailyMenus =w.DailyMenus.OrderBy(d =>d.Date)
                                    .Select(d =>
                                        new DailyMenuIndexViewModel
                                        {
                                            Date =d.Date,
                                            Calories = d.Meals
                                                    .Sum(m =>
                                                        m.Dish.Calories),
                                            PreparationTime = d.Meals
                                                    .Sum(m =>
                                                        m.Dish.PreparationTime),
                                            TotalMeals =d.Meals.Count,
                                            Portions = d.Meals.Sum(m =>m.Dish.Portions),
                                            BreakfastMeals = d.Meals
                                                    .Where(m =>
                                                        m.MealType ==
                                                        MealType.Breakfast)
                                                    .Select(m =>
                                                        new DailyMenuMealViewModel
                                                        {
                                                            MealId = m.Id,
                                                            DishId = m.DishId,
                                                            DishName =m.Dish.Name,
                                                            Description = m.Dish.Description,
                                                            DishType =m.Dish.DishType.ToString(),
                                                            MealType = m.MealType.ToString(),
                                                            Calories = m.Dish.Calories,
                                                            PreparationTime = m.Dish .PreparationTime,
                                                            Portions = m.Dish.Portions,
                                                            ImageUrl =m.Dish.ImageUrl
                                                        })
                                                    .ToList(),
                                            LunchMeals =
                                                d.Meals
                                                    .Where(m =>
                                                        m.MealType ==
                                                        MealType.Lunch)
                                                    .Select(m =>
                                                        new DailyMenuMealViewModel
                                                        {
                                                            MealId = m.Id,
                                                            DishId = m.DishId,
                                                            DishName = m.Dish.Name,
                                                            Description =m.Dish.Description,
                                                            DishType = m.Dish.DishType.ToString(),
                                                            MealType =  m.MealType.ToString(),
                                                            Calories =m.Dish.Calories,
                                                            PreparationTime = m.Dish.PreparationTime,
                                                            Portions =m.Dish.Portions,
                                                            ImageUrl =m.Dish.ImageUrl
                                                        })
                                                    .ToList(),
                                            DinnerMeals =d.Meals
                                                    .Where(m =>
                                                        m.MealType ==
                                                        MealType.Dinner)
                                                    .Select(m =>
                                                        new DailyMenuMealViewModel
                                                        {
                                                            MealId =  m.Id,
                                                            DishId =m.DishId,
                                                            DishName = m.Dish.Name,
                                                            Description =m.Dish.Description,
                                                            DishType =m.Dish.DishType.ToString(),
                                                            MealType = m.MealType.ToString(),
                                                            Calories = m.Dish.Calories,
                                                            PreparationTime =m.Dish.PreparationTime,
                                                            Portions = m.Dish.Portions,
                                                            ImageUrl = m.Dish.ImageUrl
                                                        })
                                                    .ToList()
                                        })
                                    .ToList()
                        })
                    .ToList();

            List<WeeklyMenuIndexViewModel> allWeeks =
                new List<WeeklyMenuIndexViewModel>();

            for (int weekNumber = 1;weekNumber <= weeksInYear;weekNumber++)
            {
                DateTime weekStart =
                    ISOWeek.ToDateTime(
                        year,
                        weekNumber,
                        DayOfWeek.Monday);


                WeeklyMenuIndexViewModel? existingWeek = plannedWeeks.FirstOrDefault(w =>
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


        public async Task<WeeklyMenuIndexViewModel?> GetCurrentWeekAsync(string userId)
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


            List<WeeklyMenuIndexViewModel> weeklyMenus =await GetAllAsync(currentYear,userId);


            WeeklyMenuIndexViewModel? currentWeek =
                    weeklyMenus
                        .FirstOrDefault(w =>
                            w.StartDate.Date ==
                            currentWeekStart.Date);


            return currentWeek;
        }
    }
}
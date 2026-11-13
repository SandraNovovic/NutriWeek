using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Data.Models;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Services.Core.Mappers;
using NutriWeek.Web.ViewModels.DailyMenus;
using NutriWeek.Web.ViewModels.WeeklyMenus;
using System.Globalization;

namespace NutriWeek.Services.Core.Services
{
    public class WeeklyMenuService : IWeeklyMenuService
    {
        private readonly NutriWeekDbContext _dbContext;

        public WeeklyMenuService(NutriWeekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WeeklyMenuIndexViewModel>> GetAllAsync(int year, string userId)
        {
            int weeksInYear = ISOWeek.GetWeeksInYear(year);
            DateTime firstWeekStart = ISOWeek.ToDateTime(year, 1, DayOfWeek.Monday);
            DateTime lastWeekStart = ISOWeek.ToDateTime(year, weeksInYear, DayOfWeek.Monday);

            List<WeeklyMenu> weeklyMenusFromDb =
                await _dbContext.WeeklyMenus
                    .AsNoTracking()
                    .Include(w => w.DailyMenus)
                        .ThenInclude(d => d.Meals)
                        .ThenInclude(m => m.Dish)
                    .Where(w =>
                        w.UserId == userId &&
                        w.WeekStartDate >= firstWeekStart &&
                        w.WeekStartDate <= lastWeekStart)
                    .OrderBy(w => w.WeekStartDate)
                    .ToListAsync();

            List<WeeklyMenuIndexViewModel> plannedWeeks = weeklyMenusFromDb
                .Select(MapWeeklyMenu)
                .ToList();

            List<WeeklyMenuIndexViewModel> allWeeks = new();

            for (int weekNumber = 1; weekNumber <= weeksInYear; weekNumber++)
            {
                DateTime weekStart =
                    ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday);

                WeeklyMenuIndexViewModel? existingWeek =
                    plannedWeeks.FirstOrDefault(w => w.StartDate.Date == weekStart.Date);

                allWeeks.Add(existingWeek ?? CreateEmptyWeek(weekStart));
            }

            return allWeeks;
        }

        public async Task<WeeklyMenuIndexViewModel?> GetCurrentWeekAsync(string userId)
        {
            DateTime today = DateTime.Today;
            int currentYear = ISOWeek.GetYear(today);
            int currentWeekNumber = ISOWeek.GetWeekOfYear(today);
            DateTime currentWeekStart =
                ISOWeek.ToDateTime(currentYear, currentWeekNumber, DayOfWeek.Monday);

            List<WeeklyMenuIndexViewModel> weeklyMenus =
                await GetAllAsync(currentYear, userId);

            return weeklyMenus
                .FirstOrDefault(w => w.StartDate.Date == currentWeekStart.Date);
        }

        private static WeeklyMenuIndexViewModel MapWeeklyMenu(WeeklyMenu weeklyMenu)
        {
            List<DailyMenuIndexViewModel> dailyMenus = weeklyMenu.DailyMenus
                .OrderBy(d => d.Date)
                .Select(MenuViewModelMapper.MapDailyMenu)
                .ToList();

            var meals = weeklyMenu.DailyMenus.SelectMany(d => d.Meals);

            return new WeeklyMenuIndexViewModel
            {
                Id = weeklyMenu.Id,
                StartDate = weeklyMenu.WeekStartDate,
                TotalCalories = meals.Sum(m => m.Dish.Calories),
                TotalPreparationTime = meals.Sum(m => m.Dish.PreparationTime),
                TotalMeals = meals.Count(),
                TotalPortions = meals.Sum(m => m.Dish.Portions),
                DailyMenus = dailyMenus
            };
        }

        private static WeeklyMenuIndexViewModel CreateEmptyWeek(DateTime weekStart)
        {
            return new WeeklyMenuIndexViewModel
            {
                Id = 0,
                StartDate = weekStart,
                TotalCalories = 0,
                TotalPreparationTime = 0,
                TotalMeals = 0,
                TotalPortions = 0,
                DailyMenus = new List<DailyMenuIndexViewModel>()
            };
        }
    }
}

using Microsoft.EntityFrameworkCore;
using NutriWeek.Data;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Services.Core.Mappers;
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

        public async Task<List<DailyMenuIndexViewModel>> GetAllAsync(string userId)
        {
            var dailyMenus = await _dbContext.DailyMenus
                .AsNoTracking()
                .Include(d => d.Meals)
                    .ThenInclude(m => m.Dish)
                .Where(d => d.WeeklyMenu.UserId == userId)
                .OrderBy(d => d.Date)
                .ToListAsync();

            return dailyMenus
                .Select(MenuViewModelMapper.MapDailyMenu)
                .ToList();
        }
    }
}

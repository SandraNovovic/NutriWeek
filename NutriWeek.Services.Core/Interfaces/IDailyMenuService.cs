using NutriWeek.Web.ViewModels.DailyMenus;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IDailyMenuService
    {
        Task<List<DailyMenuIndexViewModel>> GetAllAsync(string userId);
    }
}

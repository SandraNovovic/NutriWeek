using NutriWeek.Services.Core.Data.DailyMenu.Response;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IDailyMenuService
    {
        Task<List<GetAllDailyMenusResponseModel>> GetAllAsync( string userId);
    }
}
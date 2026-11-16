using NutriWeek.Services.Core.Data.WeeklyMenu.Response;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IWeeklyMenuService
    {
        Task<List<WeeklyMenuResponseModel>> GetAllAsync( int year,string userId);

        Task<WeeklyMenuResponseModel?> GetCurrentWeekAsync( string userId);
    }
}
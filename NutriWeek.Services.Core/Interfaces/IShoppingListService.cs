using NutriWeek.Services.Core.Data.ShoppingList.Response;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IShoppingListService
    {
        Task<ShoppingListResponseModel?> GetForWeekAsync( int weeklyMenuId,string userId);

        Task<List<ShoppingListItemResponseModel>?> GetForDayAsync( DateTime date,string userId);
    }
}
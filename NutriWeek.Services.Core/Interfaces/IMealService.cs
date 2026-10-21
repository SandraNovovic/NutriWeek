using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Meals;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IMealService
    {
        Task<List<MealIndexViewModel>> GetAllAsync(string userId);

        Task<IEnumerable<DropdownViewModel>> LoadDishesAsync( string userId);

        Task<bool> AddMealAsync( AddMealViewModel model, string userId);

        Task<AddMealViewModel?> GetMealForEditAsync(int id,string userId);

        bool IsMealTypeValid(MealType mealType);

        Task<bool> DishExistsAsync( int dishId, string userId);

        Task<bool> EditMealAsync( int id, AddMealViewModel meal,string userId);

        Task<DeleteMealViewModel?> GetMealForDeleteAsync( int id,string userId);

        Task<bool> DeleteMealAsync( int id, string userId);

        Task<MealDetailsViewModel?> GetMealDetailsAsync(int id, string userId);
    }
}

using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Meal.Request;
using NutriWeek.Services.Core.Data.Meal.Response;
using NutriWeek.Web.ViewModels;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IMealService
    {
        Task<List<GetAllMealsResponseModel>> GetAllAsync( string userId);


        Task<IEnumerable<DropdownResponseModel>> LoadAvailableDishesAsync( string userId);


        IEnumerable<DropdownResponseModel> LoadMealTypes();


        Task<MealServiceStatusResponseModel> AddMealAsync(CreateMealRequestData request, string userId);


        Task<GetMealForEditResponseModel?> GetMealForEditAsync(  int id, string userId);


        bool IsMealTypeValid(MealType mealType);


        Task<bool> DishExistsAsync(  int dishId, string userId);


        Task<MealServiceStatusResponseModel> EditMealAsync(  int id, EditMealRequestData request, string userId);


        Task<GetMealByIdResponseModel?> GetMealForDeleteAsync(  int id, string userId);


        Task<MealServiceStatusResponseModel> DeleteMealAsync( int id, string userId);


        Task<GetMealDetailsResponseModel?> GetMealDetailsAsync(  int id,string userId);
    }
}
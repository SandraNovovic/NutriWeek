using NutriWeek.Data.Models.Enums;
using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Dish.Request;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Web.ViewModels;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IDishService
    {
        Task<IEnumerable<GetAllDishesResponseModel>> GetAllAsync(string userId);


        Task<GetDishesPageResponseModel> GetPagedAsync(
            GetDishesPageRequestData request,
            string userId);


        IEnumerable<DropdownResponseModel> LoadDishTypes();


        Task<DishServiceStatusResponseModel> AddDishAsync(
            CreateDishRequestData request,
            string userId,
            bool isShared = false);


        Task<GetDishForEditResponseModel?> GetDishForEditAsync( int id, string userId);


        bool IsDishTypeValid(  DishType dishType);


        Task<DishServiceStatusResponseModel> EditDishAsync( int id, EditDishRequestData request, string userId);


        Task<GetDishByIdResponseModel?> GetDishForDeleteAsync( int id,string userId);


        Task<DishServiceStatusResponseModel> DeleteDishAsync( int id, string userId);


        Task<GetDishDetailsResponseModel?> GetDishDetailsAsync( int id, string userId);


        Task<IEnumerable<GetAllDishesResponseModel>> GetSharedDishesAsync();


        Task<DishServiceStatusResponseModel> SetSharedDishDeletedStatusAsync(
            int id,
            bool isDeleted);
    }
}
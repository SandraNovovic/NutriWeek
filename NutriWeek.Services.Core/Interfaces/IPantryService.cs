using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Pantry.Request;
using NutriWeek.Services.Core.Data.Pantry.Response;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IPantryService
    {
        Task<List<PantryItemResponseModel>> GetAllAsync( string userId);

        Task<List<DropdownResponseModel>> SearchIngredientsAsync(string search);

        Task<PantryServiceStatusResponseModel> AddAsync( CreatePantryItemRequestData request,string userId);

        Task<PantryServiceStatusResponseModel> EditAsync( int id, EditPantryItemRequestData request, string userId);

        Task<PantryServiceStatusResponseModel> DeleteAsync(int id,string userId);

        IEnumerable<DropdownResponseModel> LoadMeasurementUnits();
    }
}
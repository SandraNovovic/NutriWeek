using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutriWeek.Web.ViewModels.Meals;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IMealService
    {
        Task<List<MealIndexViewModel>> GetAllAsync();

        Task<IEnumerable<DropdownViewModel>> LoadDishesAsync();

        Task<bool> DishExistsAsync(int dishId);
       IEnumerable<DropdownViewModel> LoadMealTypes();

        Task<bool> AddMealAsync(AddMealViewModel model);

        Task<AddMealViewModel?> GetMealForEditAsync(int id);

        bool IsMealTypeValid(MealType mealType);

        Task<bool> EditMealAsync(int id, AddMealViewModel meal);

        Task<DeleteMealViewModel?> GetMealForDeleteAsync(int id);

        Task<bool> DeleteMealAsync(int id);

        //Task<MealDetailsViewModel?> GetMealDetailsAsync(int id);
    }

}

using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IMealService
    {
        Task<List<MealIndexViewModel>> GetAllAsync();

        //Task<IEnumerable<DropdownViewModel>> LoadDishesAsync();

        //Task<IEnumerable<DropdownViewModel>> LoadMealsAsync();

        //Task<bool> AddMealAsync(AddMealViewModel model);

        //Task<AddMealViewModel?> GetMealForEditAsync(int id);

        //bool IsMealTypeValid(MealType mealType);

        //Task<bool> DishExistsAsync(int dishId);

        //Task<bool> EditMealAsync(int id,AddMealViewModel meal);

        //Task<DeleteMealViewModel?> GetMealForDeleteAsync(int id);

        //Task<bool> DeleteMealAsync(int id);

        //Task<MealDetailsViewModel?> GetMealDetailsAsync(int id);
    }

}

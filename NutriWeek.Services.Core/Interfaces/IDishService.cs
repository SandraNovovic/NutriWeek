using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IDishService
    {
        Task<IEnumerable<DishIndexViewModel>> GetAllAsync();

        IEnumerable<DropdownViewModel> LoadDishTypes();

        Task AddDishAsync(AddDishViewModel model);

        Task<AddDishViewModel?> GetDishForEditAsync(int id);

        bool IsDishTypeValid(DishType dishType);

        Task<bool> EditDishAsync(int id,AddDishViewModel dish);

        Task<DeleteDishViewModel?> GetDishForDeleteAsync(int id);

        Task<bool> DeleteDishAsync(int id);

        Task<DetailsDishViewModel?> GetDishDetailsAsync(int id);


    }
}

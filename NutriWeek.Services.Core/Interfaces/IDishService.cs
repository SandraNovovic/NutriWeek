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
        Task<IEnumerable<DishIndexViewModel>> GetAllAsync(string userId);

        IEnumerable<DropdownViewModel> LoadDishTypes();

        Task AddDishAsync(
            AddDishViewModel model,
            string userId);

        Task<AddDishViewModel?> GetDishForEditAsync(
            int id,
            string userId);

        bool IsDishTypeValid(DishType dishType);

        Task<bool> EditDishAsync(
            int id,
            AddDishViewModel dish,
            string userId);

        Task<DeleteDishViewModel?> GetDishForDeleteAsync(
            int id,
            string userId);

        Task<bool> DeleteDishAsync(
            int id,
            string userId);

        Task<DetailsDishViewModel?> GetDishDetailsAsync(
            int id,
            string userId);
    }

}

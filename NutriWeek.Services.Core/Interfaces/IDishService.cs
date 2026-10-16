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



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NutriWeek.Web.ViewModels.WeeklyMenus;

namespace NutriWeek.Services.Core.Interfaces
{
    public interface IWeeklyMenuService
    {
        Task<List<WeeklyMenuIndexViewModel>> GetAllAsync(int year,string userId);

        Task<WeeklyMenuIndexViewModel?> GetCurrentWeekAsync(string userId);
    }
}

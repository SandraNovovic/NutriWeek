using NutriWeek.Web.ViewModels.DailyMenus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Web.ViewModels.WeeklyMenus
{
    public class WeeklyMenuIndexViewModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate => StartDate.AddDays(6);

        public int TotalCalories { get; set; }

        public int TotalPreparationTime { get; set; }

        public int TotalMeals { get; set; }

        public int TotalPortions { get; set; }

        public IEnumerable<DailyMenuIndexViewModel> DailyMenus { get; set; } = new List<DailyMenuIndexViewModel>();


    }
}

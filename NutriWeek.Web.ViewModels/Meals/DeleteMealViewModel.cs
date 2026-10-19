using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Web.ViewModels.Meals
{
    public class DeleteMealViewModel
    {
        public int Id { get; set; }

        public string DishName { get; set; } = null!;

        public DateTime Date { get; set; }

        public string MealType { get; set; } = null!;

        public string? ImageUrl { get; set; }
    }
}

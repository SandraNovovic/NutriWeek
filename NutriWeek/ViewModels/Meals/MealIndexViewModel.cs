using NutriWeek.Data.Models.Enums;

namespace NutriWeek.ViewModels.Meals
{
    public class MealIndexViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string DishName { get; set; } = null!;

        public int Calories { get; set; }

        public int Preparation { get; set; }

        public MealType MealType { get; set; }
    }
}

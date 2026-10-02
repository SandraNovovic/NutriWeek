using NutriWeek.Data.Models.Enums;

namespace NutriWeek.ViewModels.Meals
{
    public class AddMealViewModel
    {
        public int DishId { get; set; }
        public DateTime? Date { get; set; }
        public MealType MealType { get; set; }

        public IEnumerable<DropdownViewModel> Dishes { get; set; } = new List<DropdownViewModel>();

    }
}

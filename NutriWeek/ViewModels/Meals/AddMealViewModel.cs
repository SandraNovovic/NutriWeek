using NutriWeek.Data.Models.Enums;

namespace NutriWeek.ViewModels.Meals
{
    public class AddMealViewModel
    {
        public int DishId { get; set; }
        public int DailyMenuId { get; set; }
        public MealType MealType { get; set; }

        public IEnumerable<DropdownViewModel> Dishes { get; set; } = new List<DropdownViewModel>();

        public IEnumerable<DropdownViewModel> DailyMenus { get; set; } = new List<DropdownViewModel>();
    }
}

namespace NutriWeek.ViewModels.DailyMenus
{
    public class DailyMenuMealViewModel
    {
        public int MealId { get; set; } 
        public int DishId { get; set; }

        public string DishName { get; set; } = null!;

        public string Description { get; set; } = null!;    

        public string DishType { get; set; } = null!;

        public string MealType { get; set; } = null!;   

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }
    }
}

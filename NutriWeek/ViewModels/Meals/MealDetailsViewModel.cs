namespace NutriWeek.ViewModels.Meals
{
    public class MealDetailsViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string DishName { get; set; }
            = null!;

        public string? Description { get; set; }

        public string DishType { get; set; }
            = null!;

        public string MealType { get; set; }
            = null!;

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }
    }
}
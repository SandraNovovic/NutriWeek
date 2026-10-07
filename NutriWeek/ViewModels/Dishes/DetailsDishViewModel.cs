using NutriWeek.Data.Models.Enums;

namespace NutriWeek.ViewModels.Dishes
{
    public class DetailsDishViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Ingredients { get; set; } = null!;

        public string Instructions { get; set; } = null!;

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }

        public DishType DishType { get; set; }
    }
}

using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Areas.Admin.ViewModels
{
    public class SharedDishViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DishType DishType { get; set; }

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}

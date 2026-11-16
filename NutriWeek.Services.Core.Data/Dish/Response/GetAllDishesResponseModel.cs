using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Dish.Response
{
    public class GetAllDishesResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }

        public DishType DishType { get; set; }

        public bool IsShared { get; set; }
    }
}
using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Meal.Response
{
    public class GetMealByIdResponseModel
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public string DishName { get; set; } = null!;

        public DateTime Date { get; set; }

        public MealType MealType { get; set; }

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }
    }
}
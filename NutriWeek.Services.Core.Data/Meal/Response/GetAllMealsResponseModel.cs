using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Meal.Response
{
    public class GetAllMealsResponseModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string DishName { get; set; } = null!;

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public MealType MealType { get; set; }
    }
}
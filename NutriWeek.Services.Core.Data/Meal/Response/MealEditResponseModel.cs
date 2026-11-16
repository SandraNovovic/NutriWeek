using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Meal.Response
{
    public class GetMealForEditResponseModel
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public DateTime Date { get; set; }

        public MealType MealType { get; set; }
    }
}
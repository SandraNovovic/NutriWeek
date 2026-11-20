using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Dish.Response
{
    public class DishIngredientResponseModel
    {
        public string Name { get; set; } = null!;

        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
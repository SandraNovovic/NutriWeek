using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data.Models
{
    public class DishIngredient
    {
        public int Id { get; set; }


        public int DishId { get; set; }

        public Dish Dish { get; set; } = null!;


        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; } = null!;


        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
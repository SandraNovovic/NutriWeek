using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data.Models
{
    public class PantryItem
    {
        public int Id { get; set; }


        public string UserId { get; set; } = null!;


        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; } = null!;


        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
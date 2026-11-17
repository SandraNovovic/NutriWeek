using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Pantry.Response
{
    public class PantryItemResponseModel
    {
        public int Id { get; set; }

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
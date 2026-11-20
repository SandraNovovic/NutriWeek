using System.ComponentModel.DataAnnotations;

using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Pantry.Request
{
    public class CreatePantryItemRequestData
    {
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        [Range(0.01, 100000)]
        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
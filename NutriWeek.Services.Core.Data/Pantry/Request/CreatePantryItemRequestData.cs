using System.ComponentModel.DataAnnotations;

using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Pantry.Request
{
    public class CreatePantryItemRequestData
    {
        [Required]
        [StringLength(100)]
        public string IngredientName { get; set; } = null!;

        [Range(0.01, 100000)]
        public decimal Quantity { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
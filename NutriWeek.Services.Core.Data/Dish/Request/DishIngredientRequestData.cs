using System.ComponentModel.DataAnnotations;

using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Dish.Request
{
    public class DishIngredientRequestData
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Range(0.01, 100000)]
        public decimal Quantity { get; set; }

        [EnumDataType(typeof(MeasurementUnit))]
        public MeasurementUnit Unit { get; set; }
    }
}
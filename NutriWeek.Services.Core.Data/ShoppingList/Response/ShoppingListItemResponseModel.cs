using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.ShoppingList.Response
{
    public class ShoppingListItemResponseModel
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public decimal RequiredQuantity { get; set; }

        public decimal PantryQuantity { get; set; }

        public decimal QuantityToBuy { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}
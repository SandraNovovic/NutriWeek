using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Web.ViewModels.ShoppingList
{
    public class ShoppingListPageViewModel
    {
        public bool IsWeekly { get; set; }

        public int? WeeklyMenuId { get; set; }

        public DateTime WeekStartDate { get; set; }

        public DateTime? Date { get; set; }

        public string? From { get; set; }

        public List<ShoppingListItemViewModel> Items { get; set; }
            = new List<ShoppingListItemViewModel>();
    }


    public class ShoppingListItemViewModel
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public decimal RequiredQuantity { get; set; }

        public decimal PantryQuantity { get; set; }

        public decimal QuantityToBuy { get; set; }

        public MeasurementUnit Unit { get; set; }
    }
}

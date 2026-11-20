namespace NutriWeek.Services.Core.Data.ShoppingList.Response
{
    public class ShoppingListResponseModel
    {
        public int WeeklyMenuId { get; set; }

        public DateTime WeekStartDate { get; set; }

        public List<ShoppingListItemResponseModel> Items { get; set; }
            = new List<ShoppingListItemResponseModel>();
    }
}
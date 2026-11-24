using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Services.Core.Data.Dish.Request
{
    public class GetDishesPageRequestData
    {
        public string? Search { get; set; }

        public DishType? DishType { get; set; }

        public string? CookingTime { get; set; }

        public string? Calories { get; set; }

        public string? SortBy { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 8;
    }
}

using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Web.ViewModels.Dishes
{
    public class DishesIndexViewModel
    {
        public List<DishIndexViewModel> Dishes { get; set; }
            = new List<DishIndexViewModel>();

        public string? Search { get; set; }

        public DishType? DishType { get; set; }

        public string? CookingTime { get; set; }

        public string? Calories { get; set; }

        public string? SortBy { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int VegetarianCount { get; set; }

        public int VeganCount { get; set; }

        public int MeatCount { get; set; }

        public int FishCount { get; set; }
    }
}

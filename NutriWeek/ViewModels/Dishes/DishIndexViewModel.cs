using NutriWeek.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace NutriWeek.ViewModels.Dishes
{
    public class DishesIndexViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;


        public string? Description { get; set; }

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }

        public DishType DishType { get; set; }
    }
}

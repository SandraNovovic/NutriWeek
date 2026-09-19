using NutriWeek.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace NutriWeek.Data.Models
{
    public class Meal
    {
        [Key]
        public int Id { get; set; }

        public MealType MealType { get; set; }

        public int DishId { get; set; }

        public Dish Dish { get; set; } = null!;


        public int DailyMenuId { get; set; }
        public DailyMenu DailyMenu { get; set; } = null!;
    }
}

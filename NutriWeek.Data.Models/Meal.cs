
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NutriWeek.Data.Models.Enums;
namespace NutriWeek.Data.Models
{
    public class Meal
    {
        [Key]
        public int Id { get; set; }

        public MealType MealType { get; set; } 

        [ForeignKey(nameof(Dish))]
        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;



        [ForeignKey(nameof(DailyMenu))]
        public int DailyMenuId { get; set; }
        public DailyMenu DailyMenu { get; set; } = null!;
    }
}

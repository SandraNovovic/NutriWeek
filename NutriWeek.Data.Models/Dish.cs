
using NutriWeek.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static NutriWeek.Data.Models.Enums.DishType;

namespace NutriWeek.Data.Models
{

    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public string Ingredients { get; set; } = null!;

        [Required]
        public string Instructions { get; set; } = null!;

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int Portions { get; set; }

        public string? ImageUrl { get; set; }

        public DishType DishType { get; set; } 


    }
}

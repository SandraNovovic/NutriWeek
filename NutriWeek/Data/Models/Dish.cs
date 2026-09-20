using NutriWeek.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static NutriWeek.Data.EntityValidations;

namespace NutriWeek.Data.Models
{

    public class Dish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DishNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(DishDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(DishIngredientsMaxLength)]
        public string Ingredients { get; set; }=null!;

        [Required]
        [MaxLength(DishInstructionsMaxLength)]
        public string Instructions { get; set; } = null!;

        [Range(DishCaloriesMinValue, DishCaloriesMaxValue)]
        public int Calories { get; set; }

        [Range(DishPreparationTimeMinValue,DishPreparationTimeMaxValue)]
        public int PreparationTime { get; set; }

        [Range(DishPortionsMinValue,DishPortionsMaxValue)]
        public int Portions { get; set; }

        public string? ImageUrl { get; set; }
        
        public DishType DishType { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

using NutriWeek.Data.Models.Enums;
using NutriWeek.Web.ViewModels;
using NutriWeek.Web.ViewModels.Dish;

using static NutriWeek.GCommon.EntityValidations;

namespace NutriWeek.Areas.Admin.ViewModels
{
    public class AddSharedDishViewModel
    {
        [Required]
        [StringLength(DishNameMaxLength, MinimumLength = DishNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(DishDescriptionMaxLength, MinimumLength = DishDescriptionMinLength)]
        public string? Description { get; set; }

        [Required]
        [StringLength(DishInstructionsMaxLength, MinimumLength = DishInstructionsMinLength)]
        public string Instructions { get; set; } = null!;

        [Range(DishCaloriesMinValue, DishCaloriesMaxValue)]
        public int Calories { get; set; }

        [Range(DishPreparationTimeMinValue, DishPreparationTimeMaxValue)]
        public int PreparationTime { get; set; }

        [Range(DishPortionsMinValue, DishPortionsMaxValue)]
        public int Portions { get; set; }

        [Url]
        [StringLength(DishImageUrlMaxLength)]
        public string? ImageUrl { get; set; }

        public DishType DishType { get; set; }

        public IEnumerable<DropdownViewModel> DishTypes { get; set; }
            = new List<DropdownViewModel>();

        public List<DishIngredientInputViewModel> IngredientItems { get; set; }
            = new List<DishIngredientInputViewModel>();
    }
}

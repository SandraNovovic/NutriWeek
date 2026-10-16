using NutriWeek.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Web.ViewModels.Dishes
{
    public class AddDishViewModel
    {

        [Required]
        public string Name { get; set; } = null!;


        [Required]
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

        public IEnumerable<DropdownViewModel> DishTypes { get; set; } = new List<DropdownViewModel>();
    }

}

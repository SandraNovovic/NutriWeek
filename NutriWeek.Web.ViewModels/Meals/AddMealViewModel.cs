using NutriWeek.Data.Models.Enums;
using NutriWeek.GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NutriWeek.GCommon.ErrorMessages;

namespace NutriWeek.Web.ViewModels.Meals
{
    public class AddMealViewModel
    {
        [Required(ErrorMessage = DishNotSelected)]
        [Range(1, int.MaxValue, ErrorMessage = DishNotSelected)]
        public int DishId { get; set; }

        [Required(ErrorMessage = DateNotSelected)]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = MealTypeNotSelected)]
        public MealType MealType { get; set; }

        public IEnumerable<DropdownViewModel> Dishes { get; set; } = new List<DropdownViewModel>();

        public IEnumerable<DropdownViewModel> MealTypes { get; set; } = new List<DropdownViewModel>();
    }
}

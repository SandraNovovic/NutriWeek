using NutriWeek.Data.Models.Enums;


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using static NutriWeek.GCommon.ErrorMessages;
namespace NutriWeek.Services.Core.Data.Meal.Request
{
    public class EditMealRequestData
    {
        [Required(ErrorMessage = DishNotSelected)]
        [Range(1, int.MaxValue, ErrorMessage = DishNotSelected)]
        public int DishId { get; set; }

        [Required(ErrorMessage = DateNotSelected)]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = MealTypeNotSelected)]
        public MealType MealType { get; set; }

    }
}

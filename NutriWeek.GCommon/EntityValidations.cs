using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.GCommon
{
    public static class EntityValidations
    {
        public const int DishNameMinLength = 2;
        public const int DishNameMaxLength = 50;

        public const int DishDescriptionMinLength = 5;
        public const int DishDescriptionMaxLength = 300;

        public const int DishIngredientsMinLength = 3;
        public const int DishIngredientsMaxLength = 1000;

        public const int DishInstructionsMinLength = 10;
        public const int DishInstructionsMaxLength = 2000;

        public const int DishCaloriesMinValue = 1;
        public const int DishCaloriesMaxValue = 5000;

        public const int DishPreparationTimeMinValue = 1;
        public const int DishPreparationTimeMaxValue = 1440;

        public const int DishPortionsMinValue = 1;
        public const int DishPortionsMaxValue = 20;
    }
}

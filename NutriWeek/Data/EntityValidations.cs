namespace NutriWeek.Data
{
    public static class EntityValidations
    {
        /* Dish */

        public const int DishNameMinLength = 2;
        public const int DishNameMaxLength = 50;

        public const int DishDescriptionMinLength = 10;
        public const int DishDescriptionMaxLength = 300;

        public const int DishIngredientsMinLength = 10;
        public const int DishIngredientsMaxLength = 1000;

        public const int DishInstructionsMinLength = 10;
        public const int DishInstructionsMaxLength = 2000;

        public const int DishCaloriesMinValue = 1;
        public const int DishCaloriesMaxValue = 5000;

        public const int DishPreparationTimeMinValue = 1;
        public const int DishPreparationTimeMaxValue = 1440;

        public const int DishPortionsMinValue = 1;
        public const int DishPortionsMaxValue = 20;

        /* WeekMenu */
        public const int TitleMinLenght = 1;
        public const int TitleMaxLenght = 70;

        public const int DishImageUrlMaxLength = 2048;


    }
}
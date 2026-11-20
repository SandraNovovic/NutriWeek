using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

namespace NutriWeek.Data.Seeding
{
    public static class SharedDishIngredientsSeeder
    {
        public static async Task SeedAsync(
            NutriWeekDbContext dbContext)
        {
            await AddAsync(dbContext, 1, "Chicken breast", 300, MeasurementUnit.Gram);
            await AddAsync(dbContext, 1, "Rice", 160, MeasurementUnit.Gram);
            await AddAsync(dbContext, 1, "Tomato", 150, MeasurementUnit.Gram);
            await AddAsync(dbContext, 1, "Cucumber", 150, MeasurementUnit.Gram);
            await AddAsync(dbContext, 1, "Olive oil", 20, MeasurementUnit.Milliliter);

            await AddAsync(dbContext, 2, "Pasta", 200, MeasurementUnit.Gram);
            await AddAsync(dbContext, 2, "Tomato sauce", 250, MeasurementUnit.Gram);
            await AddAsync(dbContext, 2, "Zucchini", 150, MeasurementUnit.Gram);
            await AddAsync(dbContext, 2, "Bell pepper", 150, MeasurementUnit.Gram);
            await AddAsync(dbContext, 2, "Parmesan", 40, MeasurementUnit.Gram);

            await AddAsync(dbContext, 3, "Salmon", 300, MeasurementUnit.Gram);
            await AddAsync(dbContext, 3, "Broccoli", 250, MeasurementUnit.Gram);
            await AddAsync(dbContext, 3, "Carrots", 200, MeasurementUnit.Gram);
            await AddAsync(dbContext, 3, "Olive oil", 20, MeasurementUnit.Milliliter);
            await AddAsync(dbContext, 3, "Lemon", 1, MeasurementUnit.Piece);

            await AddAsync(dbContext, 4, "Oats", 80, MeasurementUnit.Gram);
            await AddAsync(dbContext, 4, "Oat milk", 250, MeasurementUnit.Milliliter);
            await AddAsync(dbContext, 4, "Banana", 1, MeasurementUnit.Piece);
            await AddAsync(dbContext, 4, "Blueberries", 50, MeasurementUnit.Gram);
            await AddAsync(dbContext, 4, "Strawberries", 80, MeasurementUnit.Gram);
        }


        private static async Task AddAsync(
            NutriWeekDbContext dbContext,
            int dishId,
            string ingredientName,
            decimal quantity,
            MeasurementUnit unit)
        {
            bool dishExists =
                await dbContext
                    .Dishes
                    .AnyAsync(d =>
                        d.Id == dishId &&
                        d.IsShared &&
                        !d.IsDeleted);


            if (!dishExists)
            {
                return;
            }


            bool alreadyExists =
                await dbContext
                    .DishIngredients
                    .AnyAsync(di =>
                        di.DishId == dishId &&
                        di.Ingredient.Name == ingredientName);


            if (alreadyExists)
            {
                return;
            }


            Ingredient? ingredient =
                await dbContext
                    .Ingredients
                    .FirstOrDefaultAsync(i =>
                        i.Name == ingredientName);


            if (ingredient == null)
            {
                ingredient =
                    new Ingredient
                    {
                        Name = ingredientName
                    };
            }


            DishIngredient dishIngredient =
                new DishIngredient
                {
                    DishId = dishId,
                    Ingredient = ingredient,
                    Quantity = quantity,
                    Unit = unit
                };


            await dbContext
                .DishIngredients
                .AddAsync(dishIngredient);


            await dbContext
                .SaveChangesAsync();
        }
    }
}

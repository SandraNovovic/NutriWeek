using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core.Data.ShoppingList.Response;
using NutriWeek.Services.Core.Interfaces;

namespace NutriWeek.Services.Core.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IWeeklyMenuRepository weeklyMenuRepository;
        private readonly IPantryItemRepository pantryItemRepository;


        public ShoppingListService(
            IWeeklyMenuRepository weeklyMenuRepository,
            IPantryItemRepository pantryItemRepository)
        {
            this.weeklyMenuRepository =
                weeklyMenuRepository;

            this.pantryItemRepository =
                pantryItemRepository;
        }


        public async Task<ShoppingListResponseModel?> GetForWeekAsync(
            int weeklyMenuId,
            string userId)
        {
            WeeklyMenu? weeklyMenu =
                await weeklyMenuRepository
                    .GetAllAsNoTracking()
                    .Include(w => w.DailyMenus)
                        .ThenInclude(d => d.Meals)
                            .ThenInclude(m => m.Dish)
                                .ThenInclude(d => d.DishIngredients)
                                    .ThenInclude(di => di.Ingredient)
                    .FirstOrDefaultAsync(w =>
                        w.Id == weeklyMenuId &&
                        w.UserId == userId &&
                        !w.IsDeleted);


            if (weeklyMenu == null)
            {
                return null;
            }


            List<PantryItem> pantryItems =
                await pantryItemRepository
                    .GetAllAsNoTracking()
                    .Where(p =>
                        p.UserId == userId)
                    .ToListAsync();


            List<DishIngredient> ingredients =
                weeklyMenu
                    .DailyMenus
                    .Where(d =>
                        !d.IsDeleted)
                    .SelectMany(d =>
                        d.Meals)
                    .Where(m =>
                        !m.IsDeleted)
                    .SelectMany(m =>
                        m.Dish.DishIngredients)
                    .ToList();


            List<ShoppingListItemResponseModel> items =
                new List<ShoppingListItemResponseModel>();


            foreach (DishIngredient ingredient in ingredients)
            {
                MeasurementUnit baseUnit =
                    GetBaseUnit(ingredient.Unit);

                decimal requiredQuantity =
                    ConvertToBaseQuantity(
                        ingredient.Quantity,
                        ingredient.Unit);


                ShoppingListItemResponseModel? existingItem =
                    items.FirstOrDefault(i =>
                        i.IngredientId ==
                            ingredient.IngredientId &&
                        i.Unit ==
                            baseUnit);


                if (existingItem == null)
                {
                    existingItem =
                        new ShoppingListItemResponseModel
                        {
                            IngredientId =
                                ingredient.IngredientId,

                            IngredientName =
                                ingredient.Ingredient.Name,

                            RequiredQuantity =
                                0,

                            PantryQuantity =
                                0,

                            QuantityToBuy =
                                0,

                            Unit =
                                baseUnit
                        };


                    items.Add(existingItem);
                }


                existingItem.RequiredQuantity +=
                    requiredQuantity;
            }


            foreach (ShoppingListItemResponseModel item in items)
            {
                PantryItem? pantryItem =
                    pantryItems.FirstOrDefault(p =>
                        p.IngredientId ==
                            item.IngredientId &&
                        GetBaseUnit(p.Unit) ==
                            item.Unit);


                if (pantryItem != null)
                {
                    item.PantryQuantity =
                        ConvertToBaseQuantity(
                            pantryItem.Quantity,
                            pantryItem.Unit);
                }


                item.QuantityToBuy =
                    item.RequiredQuantity -
                    item.PantryQuantity;


                if (item.QuantityToBuy < 0)
                {
                    item.QuantityToBuy = 0;
                }
            }


            return new ShoppingListResponseModel
            {
                WeeklyMenuId =
                    weeklyMenu.Id,

                WeekStartDate =
                    weeklyMenu.WeekStartDate,

                Items =
                    items
                        .OrderBy(i =>
                            i.IngredientName)
                        .ToList()
            };
        }


        private static decimal ConvertToBaseQuantity(
            decimal quantity,
            MeasurementUnit unit)
        {
            if (unit == MeasurementUnit.Kilogram)
            {
                return quantity * 1000;
            }


            if (unit == MeasurementUnit.Liter)
            {
                return quantity * 1000;
            }


            return quantity;
        }


        private static MeasurementUnit GetBaseUnit(
            MeasurementUnit unit)
        {
            if (unit == MeasurementUnit.Kilogram)
            {
                return MeasurementUnit.Gram;
            }


            if (unit == MeasurementUnit.Liter)
            {
                return MeasurementUnit.Milliliter;
            }


            return unit;
        }
    }
}
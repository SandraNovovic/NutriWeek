using Microsoft.EntityFrameworkCore;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core.Data.ShoppingList.Response;
using NutriWeek.Services.Core.Interfaces;

namespace NutriWeek.Services.Core.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly IWeeklyMenuRepository _weeklyMenuRepository;
        private readonly IPantryItemRepository _pantryItemRepository;
        private readonly IDailyMenuRepository _dailyMenuRepository;

        public ShoppingListService(
            IWeeklyMenuRepository weeklyMenuRepository,
            IDailyMenuRepository dailyMenuRepository,
            IPantryItemRepository pantryItemRepository)
        {
            _weeklyMenuRepository =
                weeklyMenuRepository;

            _dailyMenuRepository =
                dailyMenuRepository;

            _pantryItemRepository =
                pantryItemRepository;
        }


        public async Task<ShoppingListResponseModel?> GetForWeekAsync( int weeklyMenuId,string userId)
        {
            WeeklyMenu? weeklyMenu =
                await _weeklyMenuRepository
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
                await _pantryItemRepository
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


        private static decimal ConvertToBaseQuantity(decimal quantity, MeasurementUnit unit)
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


        private static MeasurementUnit GetBaseUnit( MeasurementUnit unit)
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
        public async Task<List<ShoppingListItemResponseModel>?> GetForDayAsync(DateTime date,string userId)
        {
            DateTime startDate = date.Date;

            DateTime endDate =startDate.AddDays(1);


            DailyMenu? dailyMenu =
                await _dailyMenuRepository
                    .GetAllAsNoTracking()
                    .Include(d => d.WeeklyMenu)
                    .Include(d => d.Meals)
                        .ThenInclude(m => m.Dish)
                            .ThenInclude(d => d.DishIngredients)
                                .ThenInclude(di => di.Ingredient)
                    .FirstOrDefaultAsync(d =>
                        d.Date >= startDate &&
                        d.Date < endDate &&
                        !d.IsDeleted &&
                        d.WeeklyMenu.UserId == userId);


            if (dailyMenu == null)
            {
                return null;
            }


            List<PantryItem> pantryItems =
                await _pantryItemRepository
                    .GetAllAsNoTracking()
                    .Where(p =>
                        p.UserId == userId)
                    .ToListAsync();


            List<ShoppingListItemResponseModel> items =new List<ShoppingListItemResponseModel>();


            foreach (Meal meal in dailyMenu.Meals)
            {
                if (meal.IsDeleted)
                {
                    continue;
                }


                foreach (DishIngredient dishIngredient
                         in meal.Dish.DishIngredients)
                {
                    MeasurementUnit unit =  GetBaseUnit(dishIngredient.Unit);


                    decimal quantity =ConvertToBaseQuantity( dishIngredient.Quantity,dishIngredient.Unit);


                    ShoppingListItemResponseModel? item =
                        items.FirstOrDefault(i =>
                            i.IngredientId ==
                                dishIngredient.IngredientId &&
                            i.Unit ==
                                unit);


                    if (item == null)
                    {
                        item =
                            new ShoppingListItemResponseModel
                            {
                                IngredientId =
                                    dishIngredient.IngredientId,

                                IngredientName =
                                    dishIngredient.Ingredient.Name,

                                Unit =
                                    unit
                            };


                        items.Add(item);
                    }


                    item.RequiredQuantity += quantity;
                }
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


                item.QuantityToBuy =item.RequiredQuantity - item.PantryQuantity;


                if (item.QuantityToBuy < 0)
                {
                    item.QuantityToBuy = 0;
                }
            }


            return items
                .OrderBy(i =>i.IngredientName)
                .ToList();
        }

    }
}
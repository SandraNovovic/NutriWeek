using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Data.Models.Enums;

using NutriWeek.Services.Core.Data.Pantry.Request;
using NutriWeek.Services.Core.Data.Pantry.Response;
using NutriWeek.Services.Core.Data.ShoppingList.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels.ShoppingList;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private readonly IShoppingListService _shoppingListService;
        private readonly IPantryService _pantryService;


        public ShoppingListController(
            IShoppingListService shoppingListService,
            IPantryService pantryService)
        {
            _shoppingListService =
                shoppingListService;

            _pantryService =
                pantryService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            int? weeklyMenuId,
            DateTime? date)
        {
            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            if (weeklyMenuId.HasValue &&
                weeklyMenuId.Value > 0)
            {
                ShoppingListResponseModel? response =
                    await _shoppingListService
                        .GetForWeekAsync(
                            weeklyMenuId.Value,
                            userId);


                if (response == null)
                {
                    return NotFound();
                }


                ShoppingListPageViewModel model =
                    new ShoppingListPageViewModel
                    {
                        IsWeekly =
                            true,

                        WeeklyMenuId =
                            response.WeeklyMenuId,

                        WeekStartDate =
                            response.WeekStartDate,

                        Items =
                            response.Items
                                .Select(MapItem)
                                .ToList()
                    };


                return View(model);
            }


            if (date.HasValue)
            {
                DateTime selectedDate =
                    date.Value.Date;


                List<ShoppingListItemResponseModel>? response =
                    await _shoppingListService
                        .GetForDayAsync(
                            selectedDate,
                            userId);


                if (response == null)
                {
                    return NotFound();
                }


                int difference =
                    ((int)selectedDate.DayOfWeek -
                     (int)DayOfWeek.Monday + 7) % 7;


                ShoppingListPageViewModel model =
                    new ShoppingListPageViewModel
                    {
                        IsWeekly =
                            false,

                        Date =
                            selectedDate,

                        WeekStartDate =
                            selectedDate
                                .AddDays(-difference),

                        Items =
                            response
                                .Select(MapItem)
                                .ToList()
                    };


                return View(model);
            }


            return RedirectToAction(
                "Index",
                "WeeklyMenus");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToPantry(
            int ingredientId,
            decimal quantity,
            MeasurementUnit unit,
            int? weeklyMenuId,
            DateTime? date)
        {
            if (ingredientId <= 0 ||
                quantity <= 0)
            {
                TempData["Error"] =
                    "The ingredient could not be moved to your pantry.";

                return RedirectToShoppingList(
                    weeklyMenuId,
                    date);
            }


            string userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!;


            CreatePantryItemRequestData request =
                new CreatePantryItemRequestData
                {
                    IngredientId =
                        ingredientId,

                    Quantity =
                        quantity,

                    Unit =
                        unit
                };


            PantryServiceStatusResponseModel response =
                await _pantryService
                    .AddPurchasedItemAsync(
                        request,
                        userId);


            if (!response.Status)
            {
                TempData["Error"] =
                    response.ErrorMessages
                        .FirstOrDefault() ??
                    "The ingredient could not be moved to your pantry.";
            }
            else
            {
                TempData["Success"] =
                    "Purchased ingredient was moved to your pantry successfully.";
            }


            return RedirectToShoppingList(
                weeklyMenuId,
                date);
        }


        private IActionResult RedirectToShoppingList(
            int? weeklyMenuId,
            DateTime? date)
        {
            if (weeklyMenuId.HasValue)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        weeklyMenuId =
                            weeklyMenuId.Value
                    });
            }


            if (date.HasValue)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        date =
                            date.Value
                                .ToString(
                                    "yyyy-MM-dd")
                    });
            }


            return RedirectToAction(
                "Index",
                "WeeklyMenus");
        }


        private static ShoppingListItemViewModel MapItem(
            ShoppingListItemResponseModel item)
        {
            return new ShoppingListItemViewModel
            {
                IngredientId =
                    item.IngredientId,

                IngredientName =
                    item.IngredientName,

                RequiredQuantity =
                    item.RequiredQuantity,

                PantryQuantity =
                    item.PantryQuantity,

                QuantityToBuy =
                    item.QuantityToBuy,

                Unit =
                    item.Unit
            };
        }
    }
}

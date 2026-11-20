using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Data.Models.Enums;

using NutriWeek.Services.Core.Data.Pantry.Request;
using NutriWeek.Services.Core.Data.Pantry.Response;
using NutriWeek.Services.Core.Data.ShoppingList.Response;
using NutriWeek.Services.Core.Data.WeeklyMenu.Response;
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
        private readonly IWeeklyMenuService _weeklyMenuService;


        public ShoppingListController(
            IShoppingListService shoppingListService,
            IPantryService pantryService,
            IWeeklyMenuService weeklyMenuService)
        {
            _shoppingListService =
                shoppingListService;

            _pantryService =
                pantryService;

            _weeklyMenuService =
                weeklyMenuService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            int? weeklyMenuId,
            DateTime? date,
            DateTime? weekStart,
            string? from)
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

                        From =
                            from,

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


                response ??=
                    new List<ShoppingListItemResponseModel>();


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

                        From =
                            from,

                        Items =
                            response
                                .Select(MapItem)
                                .ToList()
                    };


                return View(model);
            }


            if (weekStart.HasValue)
            {
                ShoppingListPageViewModel selectedWeekModel =
                    new ShoppingListPageViewModel
                    {
                        IsWeekly =
                            true,

                        WeekStartDate =
                            weekStart.Value.Date,

                        From =
                            from
                    };


                return View(selectedWeekModel);
            }


            WeeklyMenuResponseModel? currentWeek =
                await _weeklyMenuService
                    .GetCurrentWeekAsync(
                        userId);


            if (currentWeek != null &&
                currentWeek.Id > 0)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        weeklyMenuId =
                            currentWeek.Id,
                        from
                    });
            }


            ShoppingListPageViewModel emptyModel =
                new ShoppingListPageViewModel
                {
                    IsWeekly =
                        true,

                    WeekStartDate =
                        currentWeek?.StartDate ??
                        GetCurrentWeekStart(),

                    From =
                        from
                };


            return View(emptyModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToPantry(
            int ingredientId,
            decimal quantity,
            MeasurementUnit unit,
            int? weeklyMenuId,
            DateTime? date,
            string? from)
        {
            if (ingredientId <= 0 ||
                quantity <= 0)
            {
                TempData["Error"] =
                    "The ingredient could not be moved to your pantry.";

                return RedirectToShoppingList(
                    weeklyMenuId,
                    date,
                    from);
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
                date,
                from);
        }


        private IActionResult RedirectToShoppingList(
            int? weeklyMenuId,
            DateTime? date,
            string? from)
        {
            if (weeklyMenuId.HasValue)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        weeklyMenuId =
                            weeklyMenuId.Value,
                        from
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
                                    "yyyy-MM-dd"),
                        from
                    });
            }


            return RedirectToAction(
                nameof(Index),
                new
                {
                    from
                });
        }


        private static DateTime GetCurrentWeekStart()
        {
            DateTime today =
                DateTime.Today;

            int difference =
                ((int)today.DayOfWeek -
                 (int)DayOfWeek.Monday + 7) % 7;

            return today
                .AddDays(-difference)
                .Date;
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

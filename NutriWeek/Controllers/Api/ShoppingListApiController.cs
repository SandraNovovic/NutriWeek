using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.ShoppingList.Response;
using NutriWeek.Services.Core.Interfaces;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/shopping-list")]
    public class ShoppingListApiController : ControllerBase
    {
        private readonly IShoppingListService _shoppingListService;


        public ShoppingListApiController( IShoppingListService shoppingListService)
        {
            _shoppingListService =shoppingListService;
        }


        [HttpGet("week/{weeklyMenuId:int}")]
        public async Task<IActionResult> GetForWeek( int weeklyMenuId)
        {
            if (weeklyMenuId <= 0)
            {
                return BadRequest();
            }


            string userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            ShoppingListResponseModel? shoppingList =
                await _shoppingListService
                    .GetForWeekAsync(
                        weeklyMenuId,
                        userId);


            if (shoppingList == null)
            {
                return NotFound();
            }


            return Ok(shoppingList);
        }

        [HttpGet("day/{date}")]
        public async Task<IActionResult> GetForDay(DateTime date)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            List<ShoppingListItemResponseModel>? shoppingList =
                await _shoppingListService
                    .GetForDayAsync(
                        date,
                        userId);


            if (shoppingList == null)
            {
                return NotFound();
            }


            return Ok(shoppingList);
        }
    }
}
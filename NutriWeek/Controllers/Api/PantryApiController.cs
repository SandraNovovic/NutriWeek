using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NutriWeek.Services.Core.Data.Pantry.Request;
using NutriWeek.Services.Core.Data.Pantry.Response;
using NutriWeek.Services.Core.Interfaces;

using System.Security.Claims;

namespace NutriWeek.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/pantry")]
    public class PantryApiController : ControllerBase
    {
        private readonly IPantryService _pantryService;


        public PantryApiController( IPantryService pantryService)
        {
            _pantryService = pantryService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            List<PantryItemResponseModel> pantryItems =
                await _pantryService
                    .GetAllAsync(userId);


            return Ok(pantryItems);
        }


        [HttpGet("ingredients")]
        public async Task<IActionResult> SearchIngredients(
            [FromQuery] string search)
        {
            var ingredients =
                await _pantryService
                    .SearchIngredientsAsync(
                        search ?? string.Empty);


            return Ok(ingredients);
        }


        [HttpGet("units")]
        public IActionResult GetMeasurementUnits()
        {
            var units = _pantryService .LoadMeasurementUnits();


            return Ok(units);
        }


        [HttpPost]
        public async Task<IActionResult> Add( [FromBody] CreatePantryItemRequestData request)
        {
            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            PantryServiceStatusResponseModel response =
                await 
                _pantryService
                    .AddAsync(request, userId);


            if (!response.Status)
            {
                return BadRequest(
                    new
                    {
                        errors =
                            response.ErrorMessages
                    });
            }


            return Ok(
                new
                {
                    message =
                        "Pantry item added successfully."
                });
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit( int id,  [FromBody] EditPantryItemRequestData request)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;


            PantryServiceStatusResponseModel response =
                await _pantryService
                    .EditAsync(
                        id,
                        request,
                        userId);


            if (!response.Status)
            {
                bool notFound =
                    response.ErrorMessages
                        .Any(e =>
                            e ==
                            "Pantry item not found.");


                if (notFound)
                {
                    return NotFound();
                }


                return BadRequest(
                    new
                    {
                        errors =
                            response.ErrorMessages
                    });
            }


            return Ok(
                new
                {
                    message =
                        "Pantry item updated successfully."
                });
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete( int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            string userId = User.FindFirstValue( ClaimTypes.NameIdentifier)!;


            PantryServiceStatusResponseModel response =
                await _pantryService
                    .DeleteAsync( id,userId);


            if (!response.Status)
            {
                bool notFound =
                    response.ErrorMessages
                        .Any(e =>
                            e ==
                            "Pantry item not found.");


                if (notFound)
                {
                    return NotFound();
                }


                return BadRequest(
                    new
                    {
                        errors = response.ErrorMessages
                    });
            }


            return Ok(
                new
                {
                    message = "Pantry item deleted successfully."
                });
        }
    }
}
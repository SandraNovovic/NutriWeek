using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.GCommon.Utilities;

using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Pantry.Request;
using NutriWeek.Services.Core.Data.Pantry.Response;
using NutriWeek.Services.Core.Interfaces;

namespace NutriWeek.Services.Core.Services
{
    public class PantryService : IPantryService
    {
        private readonly IPantryItemRepository pantryItemRepository;
        private readonly IIngredientRepository ingredientRepository;

        private readonly BasicModelValidator modelValidator;
        private readonly ILogger<PantryService> logger;


        public PantryService(
            IPantryItemRepository pantryItemRepository,
            IIngredientRepository ingredientRepository,
            BasicModelValidator modelValidator,
            ILogger<PantryService> logger)
        {
            this.pantryItemRepository =
                pantryItemRepository;

            this.ingredientRepository =
                ingredientRepository;

            this.modelValidator =
                modelValidator;

            this.logger =
                logger;
        }


        public async Task<List<PantryItemResponseModel>> GetAllAsync(string userId)
        {
            return await pantryItemRepository
                .GetAllAsNoTracking()
                .Where(p =>
                    p.UserId == userId)
                .OrderBy(p =>
                    p.Ingredient.Name)
                .Select(p =>
                    new PantryItemResponseModel
                    {
                        Id =
                            p.Id,

                        IngredientId =
                            p.IngredientId,

                        IngredientName =
                            p.Ingredient.Name,

                        Quantity =
                            p.Quantity,

                        Unit =
                            p.Unit
                    })
                .ToListAsync();
        }


        public async Task<PantryServiceStatusResponseModel> AddAsync( CreatePantryItemRequestData request, string userId)
        {
            PantryServiceStatusResponseModel response =
                new PantryServiceStatusResponseModel();


            try
            {
                if (!modelValidator.IsValid(request))
                {
                    response.ErrorMessages =
                        modelValidator
                            .ValidationResults
                            .Select(v =>
                                v.ErrorMessage)
                            .ToArray();

                    return response;
                }


                string ingredientName =
                    request.IngredientName.Trim();


                Ingredient? ingredient =
                    await ingredientRepository
                        .GetAll()
                        .FirstOrDefaultAsync(i => i.Name == ingredientName);


                if (ingredient == null)
                {
                    ingredient =
                        new Ingredient
                        {
                            Name = ingredientName
                        };


                    await ingredientRepository.AddAsync(ingredient);

                    await ingredientRepository.SaveChangesAsync();
                }


                bool alreadyExists =
                    await pantryItemRepository
                        .GetAllAsNoTracking()
                        .AnyAsync(p =>
                            p.UserId == userId &&
                            p.IngredientId ==
                            ingredient.Id);


                if (alreadyExists)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "This ingredient is already in your pantry."
                        };

                    return response;
                }


                PantryItem pantryItem =
                    new PantryItem
                    {
                        UserId =
                            userId,

                        IngredientId =
                            ingredient.Id,

                        Quantity =
                            request.Quantity,

                        Unit =
                            request.Unit
                    };


                await pantryItemRepository.AddAsync(pantryItem);

                await pantryItemRepository .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while adding pantry item for user {UserId}.",
                    userId);


                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred. Please try again."
                    };
            }


            return response;
        }


        public async Task<PantryServiceStatusResponseModel> EditAsync( int id,EditPantryItemRequestData request, string userId)
        {
            PantryServiceStatusResponseModel response =new PantryServiceStatusResponseModel();


            try
            {
                if (!modelValidator.IsValid(request))
                {
                    response.ErrorMessages =
                        modelValidator
                            .ValidationResults
                            .Select(v =>
                                v.ErrorMessage)
                            .ToArray();

                    return response;
                }


                PantryItem? pantryItem =
                    await pantryItemRepository
                        .GetAll()
                        .FirstOrDefaultAsync(p =>
                            p.Id == id &&
                            p.UserId == userId);


                if (pantryItem == null)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Pantry item not found."
                        };

                    return response;
                }


                pantryItem.Quantity =
                    request.Quantity;

                pantryItem.Unit =
                    request.Unit;


                await pantryItemRepository
                    .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while editing pantry item {PantryItemId}.",
                    id);


                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred. Please try again."
                    };
            }


            return response;
        }


        public async Task<PantryServiceStatusResponseModel> DeleteAsync( int id,string userId)
        {
            PantryServiceStatusResponseModel response = new PantryServiceStatusResponseModel();


            try
            {
                PantryItem? pantryItem =
                    await pantryItemRepository
                        .GetAll()
                        .FirstOrDefaultAsync(p =>
                            p.Id == id &&
                            p.UserId == userId);


                if (pantryItem == null)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Pantry item not found."
                        };

                    return response;
                }


                pantryItemRepository
                    .Remove(pantryItem);

                await pantryItemRepository
                    .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while deleting pantry item {PantryItemId}.",
                    id);


                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred. Please try again."
                    };
            }


            return response;
        }


        public IEnumerable<DropdownResponseModel> LoadMeasurementUnits()
        {
            return Enum
                .GetValues<MeasurementUnit>()
                .Select(u =>
                    new DropdownResponseModel
                    {
                        Id =
                            (int)u,

                        Name =
                            u.ToString()
                    })
                .ToList();
        }
    }
}
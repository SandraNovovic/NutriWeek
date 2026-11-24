using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository;
using NutriWeek.Data.Repository.Interfaces;
using NutriWeek.GCommon.Utilities;
using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Dish.Request;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;
using DishEntity = NutriWeek.Data.Models.Dish;
namespace NutriWeek.Services.Core.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;
        private readonly BasicModelValidator _modelValidator;
        private readonly ILogger<DishService> _logger;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IDishIngredientRepository _dishIngredientRepository;

        public DishService(
            IDishRepository dishRepository,
            BasicModelValidator modelValidator,
            ILogger<DishService> logger,
            IIngredientRepository ingredientRepository,
            IDishIngredientRepository dishIngredientRepository)
        {
            _dishRepository = dishRepository;
            _modelValidator = modelValidator;
            _logger = logger;
            _ingredientRepository = ingredientRepository;
            _dishIngredientRepository = dishIngredientRepository;
        }


        public async Task<IEnumerable<GetAllDishesResponseModel>> GetAllAsync( string userId)
        {
            try
            {
                List<GetAllDishesResponseModel> dishes =
                    await _dishRepository
                        .GetAllAsNoTracking()
                        .Where(d =>
                            !d.IsDeleted &&(d.IsShared ||
                            d.UserId == userId))
                        .Select(d =>
                            new GetAllDishesResponseModel
                            {
                                Id = d.Id,
                                Name = d.Name,
                                Description = d.Description,
                                Calories = d.Calories,
                                PreparationTime =
                                    d.PreparationTime,
                                Portions = d.Portions,
                                ImageUrl = d.ImageUrl,
                                DishType = d.DishType,
                                IsShared = d.IsShared
                            })
                        .ToListAsync();

                return dishes;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading dishes for user {UserId}.",
                    userId);

                throw;
            }
        }


        public async Task<GetDishesPageResponseModel> GetPagedAsync(
            GetDishesPageRequestData request,
            string userId)
        {
            try
            {
                IQueryable<DishEntity> baseQuery =
                    _dishRepository
                        .GetAllAsNoTracking()
                        .Where(d =>
                            !d.IsDeleted &&
                            (d.IsShared ||
                             d.UserId == userId));


                GetDishesPageResponseModel response =
                    new GetDishesPageResponseModel
                    {
                        VegetarianCount =
                            await baseQuery.CountAsync(d =>
                                d.DishType ==
                                DishType.Vegetarian),

                        VeganCount =
                            await baseQuery.CountAsync(d =>
                                d.DishType ==
                                DishType.Vegan),

                        MeatCount =
                            await baseQuery.CountAsync(d =>
                                d.DishType ==
                                DishType.Meat),

                        FishCount =
                            await baseQuery.CountAsync(d =>
                                d.DishType ==
                                DishType.Fish)
                    };


                IQueryable<DishEntity> query =
                    baseQuery;


                if (!string.IsNullOrWhiteSpace(
                        request.Search))
                {
                    string search =
                        request.Search.Trim();


                    query =
                        query.Where(d =>
                            d.Name.Contains(search) ||
                            (d.Description != null &&
                             d.Description.Contains(search)));
                }


                if (request.DishType.HasValue &&
                    IsDishTypeValid(
                        request.DishType.Value))
                {
                    query =
                        query.Where(d =>
                            d.DishType ==
                            request.DishType.Value);
                }


                query =
                    request.CookingTime switch
                    {
                        "under15" =>
                            query.Where(d =>
                                d.PreparationTime < 15),

                        "15to30" =>
                            query.Where(d =>
                                d.PreparationTime >= 15 &&
                                d.PreparationTime <= 30),

                        "over30" =>
                            query.Where(d =>
                                d.PreparationTime > 30),

                        _ =>
                            query
                    };


                query =
                    request.Calories switch
                    {
                        "under350" =>
                            query.Where(d =>
                                d.Calories < 350),

                        "350to450" =>
                            query.Where(d =>
                                d.Calories >= 350 &&
                                d.Calories <= 450),

                        "451to550" =>
                            query.Where(d =>
                                d.Calories >= 451 &&
                                d.Calories <= 550),

                        "over550" =>
                            query.Where(d =>
                                d.Calories > 550),

                        _ =>
                            query
                    };


                query =
                    request.SortBy switch
                    {
                        "nameAsc" =>
                            query.OrderBy(d =>
                                d.Name),

                        "caloriesAsc" =>
                            query.OrderBy(d =>
                                d.Calories),

                        "caloriesDesc" =>
                            query.OrderByDescending(d =>
                                d.Calories),

                        _ =>
                            query.OrderByDescending(d =>
                                d.Id)
                    };


                int pageSize =
                    request.PageSize > 0
                        ? request.PageSize
                        : 8;


                response.TotalItems =
                    await query.CountAsync();


                response.TotalPages =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            response.TotalItems /
                            (double)pageSize));


                response.CurrentPage =
                    Math.Clamp(
                        request.Page,
                        1,
                        response.TotalPages);


                response.Dishes =
                    await query
                        .Skip(
                            (response.CurrentPage - 1) *
                            pageSize)
                        .Take(pageSize)
                        .Select(d =>
                            new GetAllDishesResponseModel
                            {
                                Id =
                                    d.Id,

                                Name =
                                    d.Name,

                                Description =
                                    d.Description,

                                Calories =
                                    d.Calories,

                                PreparationTime =
                                    d.PreparationTime,

                                Portions =
                                    d.Portions,

                                ImageUrl =
                                    d.ImageUrl,

                                DishType =
                                    d.DishType,

                                IsShared =
                                    d.IsShared,

                                IsDeleted =
                                    d.IsDeleted
                            })
                        .ToListAsync();


                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading filtered dishes for user {UserId}.",
                    userId);

                throw;
            }
        }


        public async Task<IEnumerable<GetAllDishesResponseModel>> GetSharedDishesAsync()
        {
            try
            {
                return await _dishRepository
                    .GetAllAsNoTracking()
                    .Where(d =>
                        d.IsShared)
                    .OrderBy(d =>
                        d.IsDeleted)
                    .ThenBy(d =>
                        d.Name)
                    .Select(d =>
                        new GetAllDishesResponseModel
                        {
                            Id =
                                d.Id,

                            Name =
                                d.Name,

                            Description =
                                d.Description,

                            Calories =
                                d.Calories,

                            PreparationTime =
                                d.PreparationTime,

                            Portions =
                                d.Portions,

                            ImageUrl =
                                d.ImageUrl,

                            DishType =
                                d.DishType,

                            IsShared =
                                d.IsShared,

                            IsDeleted =
                                d.IsDeleted
                        })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading shared dishes.");

                throw;
            }
        }


        public async Task<DishServiceStatusResponseModel> SetSharedDishDeletedStatusAsync(
            int id,
            bool isDeleted)
        {
            DishServiceStatusResponseModel response =
                new DishServiceStatusResponseModel();


            try
            {
                DishEntity? dish =
                    await _dishRepository
                        .GetAll()
                        .FirstOrDefaultAsync(d =>
                            d.Id == id &&
                            d.IsShared);


                if (dish == null)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Shared dish not found."
                        };

                    return response;
                }


                dish.IsDeleted =
                    isDeleted;


                await _dishRepository
                    .SaveChangesAsync();


                response.Status =
                    true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while changing shared dish {DishId} status.",
                    id);


                response.ErrorMessages =
                    new[]
                    {
                        "An error occurred while updating the shared dish."
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while changing shared dish {DishId} status.",
                    id);


                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred while updating the shared dish."
                    };
            }


            return response;
        }


        public IEnumerable<DropdownResponseModel> LoadDishTypes()
        {
            return Enum
                .GetValues<DishType>()
                .Select(d =>
                    new DropdownResponseModel
                    {
                        Id = (int)d,
                        Name = d.ToString()
                    })
                .ToList();
        }

        public async Task<DishServiceStatusResponseModel> AddDishAsync(
            CreateDishRequestData request,
            string userId,
            bool isShared = false)
        {
            DishServiceStatusResponseModel response =
                new DishServiceStatusResponseModel();


            bool isValid =
                _modelValidator.IsValid(request);


            if (!isValid)
            {
                response.Status = false;

                response.ErrorMessages =
                    _modelValidator
                        .ValidationResults
                        .Select(vr =>vr.ErrorMessage)
                        .ToArray();

                return response;
            }


            if (!IsDishTypeValid(request.DishType))
            {
                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        "Invalid dish type."
                    };

                return response;
            }

            bool hasDuplicateIngredients = false;

            for (int i = 0; i < request.IngredientItems.Count; i++)
            {
                for (int j = i + 1; j < request.IngredientItems.Count; j++)
                {
                    if (request.IngredientItems[i].Name.Trim().ToLower() == request.IngredientItems[j].Name.Trim().ToLower())
                    {
                        hasDuplicateIngredients = true;
                    }
                }
            }

            if (hasDuplicateIngredients)
            {
                response.ErrorMessages =
                    new[]
                    {
            "Each ingredient can only be added once to a dish."
                    };

                return response;
            }

            DishEntity dish =
                new DishEntity
                {
                    Name =
                        request.Name,

                    Description =
                        request.Description,

                    Ingredients =
                        request.Ingredients,

                    Instructions =
                        request.Instructions,

                    Calories =
                        request.Calories,

                    PreparationTime =
                        request.PreparationTime,

                    Portions =
                        request.Portions,

                    ImageUrl =
                        request.ImageUrl,

                    DishType =
                        request.DishType,

                    UserId =
                        isShared
                            ? null
                            : userId,

                    IsShared =
                        isShared
                };


            try
            {
                await _dishRepository.AddAsync( dish);

                await AddDishIngredientsAsync(
                    dish,
                    request.IngredientItems);

                await _dishRepository
                    .SaveChangesAsync();


                response.Status = true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while creating dish for user {UserId}.",
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        "An error occurred while creating the dish."
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while creating dish for user {UserId}.",
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred while creating the dish."
                    };
            }


            return response;
        }


        public async Task<GetDishForEditResponseModel?> GetDishForEditAsync(int id,string userId)
        {
            try
            {
                GetDishForEditResponseModel? dish =
                    await _dishRepository
                        .GetAllAsNoTracking()
                        .Where(d =>
                            d.Id == id &&
                            !d.IsDeleted &&
                            d.UserId == userId)
                        .Select(d =>
                            new GetDishForEditResponseModel
                            {
                                Id =
                                    d.Id,

                                Name =
                                    d.Name,

                                Description =
                                    d.Description,

                                Ingredients =
                                    d.Ingredients,

                                Instructions =
                                    d.Instructions,

                                Calories =
                                    d.Calories,

                                PreparationTime =
                                    d.PreparationTime,

                                Portions =
                                    d.Portions,

                                ImageUrl =
                                    d.ImageUrl,

                                DishType =
                                    d.DishType,

                                IngredientItems = d.DishIngredients
                                          .Select(di => new DishIngredientResponseModel
                                          {
                                              Name = di.Ingredient.Name,
                                              Quantity = di.Quantity,
                                              Unit = di.Unit
                                          })
                                          .ToList()
                            })
                        .SingleOrDefaultAsync();


                return dish;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading dish {DishId} for editing.",
                    id);

                throw;
            }
        }


        public bool IsDishTypeValid(DishType dishType)
        {
            return Enum.IsDefined(
                typeof(DishType),
                dishType);
        }


        public async Task<DishServiceStatusResponseModel> EditDishAsync(int id,EditDishRequestData request, string userId)
        {
            DishServiceStatusResponseModel response =new DishServiceStatusResponseModel();


            bool isValid = _modelValidator.IsValid(request);


            if (!isValid)
            {
                response.Status = false;

                response.ErrorMessages =
                    _modelValidator
                        .ValidationResults
                        .Select(vr =>
                            vr.ErrorMessage)
                        .ToArray();

                return response;
            }


            if (!IsDishTypeValid(request.DishType))
            {
                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                "Invalid dish type."
                    };

                return response;
            }


            bool hasDuplicateIngredients = false;


            for (int i = 0; i < request.IngredientItems.Count; i++)
            {
                for (int j = i + 1;
                     j < request.IngredientItems.Count;
                     j++)
                {
                    if (request.IngredientItems[i]
                            .Name
                            .Trim()
                            .ToLower() ==
                        request.IngredientItems[j]
                            .Name
                            .Trim()
                            .ToLower())
                    {
                        hasDuplicateIngredients = true;
                    }
                }
            }


            if (hasDuplicateIngredients)
            {
                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                "Each ingredient can only be added once to a dish."
                    };

                return response;
            }


            try
            {
                DishEntity? dishToEdit =
                    await _dishRepository
                        .GetAll()
                        .Include(d => d.DishIngredients)
                        .FirstOrDefaultAsync(d =>
                            d.Id == id &&
                            !d.IsDeleted &&
                            d.UserId == userId);


                if (dishToEdit == null)
                {
                    response.Status = false;

                    response.ErrorMessages =
                        new[]
                        {
                    "Dish not found."
                        };

                    return response;
                }


                dishToEdit.Name =
                    request.Name;

                dishToEdit.Description =
                    request.Description;

                dishToEdit.Ingredients =
                    request.Ingredients;

                dishToEdit.Instructions =
                    request.Instructions;

                dishToEdit.Calories =
                    request.Calories;

                dishToEdit.PreparationTime =
                    request.PreparationTime;

                dishToEdit.Portions =
                    request.Portions;

                dishToEdit.ImageUrl =
                    request.ImageUrl;

                dishToEdit.DishType =
                    request.DishType;


                foreach (DishIngredient ingredient
                         in dishToEdit.DishIngredients.ToList())
                {
                    _dishIngredientRepository
                        .Remove(ingredient);
                }


                await AddDishIngredientsAsync(  dishToEdit,  request.IngredientItems);


                await _dishRepository .SaveChangesAsync();


                response.Status = true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while editing dish {DishId} for user {UserId}.",
                    id,
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                "An error occurred while updating the dish."
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while editing dish {DishId} for user {UserId}.",
                    id,
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                "An unexpected error occurred while updating the dish."
                    };
            }


            return response;
        }

        public async Task<GetDishByIdResponseModel?> GetDishForDeleteAsync(int id, string userId)
        {
            try
            {
                GetDishByIdResponseModel? dish =
                    await _dishRepository
                        .GetAllAsNoTracking()
                        .Where(d =>
                            d.Id == id &&
                            !d.IsDeleted &&
                            d.UserId == userId)
                        .Select(d =>
                            new GetDishByIdResponseModel
                            {
                                Id =
                                    d.Id,

                                Name =
                                    d.Name,

                                Description =
                                    d.Description,

                                Ingredients =
                                    d.Ingredients,

                                Instructions =
                                    d.Instructions,

                                Calories =
                                    d.Calories,

                                PreparationTime =
                                    d.PreparationTime,

                                Portions =
                                    d.Portions,

                                ImageUrl =
                                    d.ImageUrl,

                                DishType =
                                    d.DishType,

                                IsShared =
                                    d.IsShared
                            })
                        .SingleOrDefaultAsync();


                return dish;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading dish {DishId} for deletion.",
                    id);

                throw;
            }
        }


        public async Task<DishServiceStatusResponseModel> DeleteDishAsync( int id, string userId)
        {
            DishServiceStatusResponseModel response = new DishServiceStatusResponseModel();


            try
            {
                DishEntity? dishToDelete =
                    await _dishRepository
                        .GetAll()
                        .FirstOrDefaultAsync(d =>
                            d.Id == id &&
                            !d.IsDeleted &&
                            d.UserId == userId);


                if (dishToDelete == null)
                {
                    response.Status = false;

                    response.ErrorMessages =
                        new[]
                        {
                            "Dish not found."
                        };

                    return response;
                }


                _dishRepository.SoftDelete( dishToDelete);


                await _dishRepository.SaveChangesAsync();


                response.Status = true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Database error while deleting dish {DishId} for user {UserId}.",
                    id,
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        "An error occurred while deleting the dish."
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while deleting dish {DishId} for user {UserId}.",
                    id,
                    userId);


                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        "An unexpected error occurred while deleting the dish."
                    };
            }


            return response;
        }


        public async Task<GetDishDetailsResponseModel?> GetDishDetailsAsync(  int id,string userId)
        {
            try
            {
                GetDishDetailsResponseModel? dish =
                    await _dishRepository
                        .GetAllAsNoTracking()
                        .Where(d =>
                            d.Id == id &&
                            !d.IsDeleted &&
                            (d.IsShared ||
                             d.UserId == userId))
                        .Select(d =>
                            new GetDishDetailsResponseModel
                            {
                                Id =
                                    d.Id,

                                Name =
                                    d.Name,

                                Description =
                                    d.Description,

                                Ingredients =
                                    d.Ingredients,

                                Instructions =
                                    d.Instructions,

                                Calories =
                                    d.Calories,

                                PreparationTime =
                                    d.PreparationTime,

                                Portions =
                                    d.Portions,

                                ImageUrl =
                                    d.ImageUrl,

                                DishType =
                                    d.DishType,

                                IsShared =
                                    d.IsShared,

                                IngredientItems =
                               d.DishIngredients
                                   .Select(di =>
                                       new DishIngredientResponseModel
                                       {
                                           Name =
                                               di.Ingredient.Name,
                              
                                           Quantity =
                                               di.Quantity,
                              
                                           Unit =
                                               di.Unit
                                       })
                                   .ToList(),
                            })
                        .SingleOrDefaultAsync();


                return dish;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while loading details for dish {DishId}.",
                    id);

                throw;
            }
        }

        private async Task AddDishIngredientsAsync(DishEntity dish, IEnumerable<DishIngredientRequestData> ingredientItems)
        {
            foreach (DishIngredientRequestData item in ingredientItems)
            {
                string ingredientName = item.Name.Trim();

                Ingredient? ingredient =
                    await _ingredientRepository
                        .GetAll()
                        .FirstOrDefaultAsync(i =>
                            i.Name == ingredientName);

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Name = ingredientName
                    };

                    await _ingredientRepository
                        .AddAsync(ingredient);
                }

                DishIngredient dishIngredient =
                    new DishIngredient
                    {
                        Dish = dish,
                        Ingredient = ingredient,
                        Quantity = item.Quantity,
                        Unit = item.Unit
                    };

                await _dishIngredientRepository
                    .AddAsync(dishIngredient);
            }
        }
    }
}
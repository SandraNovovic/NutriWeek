using NutriWeek.GCommon.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;
using NutriWeek.Data.Repository.Interfaces;
using NutriWeek.Services.Core.Data.Common.Response;
using NutriWeek.Services.Core.Data.Dish.Request;
using NutriWeek.Services.Core.Data.Dish.Response;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Web.ViewModels;

namespace NutriWeek.Services.Core.Services
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;
        private readonly BasicModelValidator _modelValidator;
        private readonly ILogger<DishService> _logger;


        public DishService(
            IDishRepository dishRepository,
            BasicModelValidator modelValidator,
            ILogger<DishService> logger)
        {
            _dishRepository = dishRepository;
            _modelValidator = modelValidator;
            _logger = logger;
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

        public async Task<DishServiceStatusResponseModel> AddDishAsync( CreateDishRequestData request,string userId)
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


            Dish dish =
                new Dish
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
                        userId,

                    IsShared =
                        false
                };


            try
            {
                _dishRepository.Add( dish);

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
                            (d.IsShared ||
                             d.UserId == userId))
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
                                    d.DishType
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


            bool isValid =
                _modelValidator.IsValid(request);


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


            try
            {
                Dish? dishToEdit =
                    await _dishRepository
                        .GetAll()
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


                await _dishRepository
                    .SaveChangesAsync();


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
                            (d.IsShared ||
                             d.UserId == userId))
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
                Dish? dishToDelete =
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
                                    d.IsShared
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
    }
}
using CinemaApp.Services.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core.Data.Meal.Request;
using NutriWeek.Services.Core.Data.Meal.Response;
using NutriWeek.Services.Core.Interfaces;

using NutriWeek.Web.ViewModels;

namespace NutriWeek.Services.Core.Implementations
{
    public class MealService : IMealService
    {
        private readonly IMealRepository mealRepository;
        private readonly IDishRepository dishRepository;
        private readonly IDailyMenuRepository dailyMenuRepository;
        private readonly IWeeklyMenuRepository weeklyMenuRepository;

        private readonly BasicModelValidator modelValidator;
        private readonly ILogger<MealService> logger;

        public MealService(
            IMealRepository mealRepository,
            IDishRepository dishRepository,
            IDailyMenuRepository dailyMenuRepository,
            IWeeklyMenuRepository weeklyMenuRepository,
            BasicModelValidator modelValidator,
            ILogger<MealService> logger)
        {
            this.mealRepository = mealRepository;
            this.dishRepository = dishRepository;
            this.dailyMenuRepository = dailyMenuRepository;
            this.weeklyMenuRepository = weeklyMenuRepository;

            this.modelValidator = modelValidator;
            this.logger = logger;
        }


        public async Task<List<GetAllMealsResponseModel>> GetAllAsync(string userId)
        {
            List<GetAllMealsResponseModel> meals =
                await mealRepository
                    .GetAllAsNoTracking()
                    .Where(m =>
                        m.DailyMenu
                            .WeeklyMenu
                            .UserId == userId)
                    .OrderBy(m =>
                        m.DailyMenu.Date)
                    .ThenBy(m =>
                        m.MealType)
                    .Select(m =>
                        new GetAllMealsResponseModel
                        {
                            Id = m.Id,
                            Date = m.DailyMenu.Date,
                            DishName = m.Dish.Name,
                            Calories = m.Dish.Calories,
                            PreparationTime =
                                m.Dish.PreparationTime,
                            MealType = m.MealType
                        })
                    .ToListAsync();

            return meals;
        }


        public async Task<IEnumerable<DropdownViewModel>> LoadDishesAsync(string userId)
        {
            IEnumerable<DropdownViewModel> dishes =
                await dishRepository
                    .GetAllAsNoTracking()
                    .Where(d =>
                        d.IsShared ||
                        d.UserId == userId)
                    .OrderBy(d => d.Name)
                    .Select(d =>
                        new DropdownViewModel
                        {
                            Id = d.Id,
                            Name = d.Name
                        })
                    .ToListAsync();

            return dishes;
        }


        public IEnumerable<DropdownViewModel> LoadMealTypes()
        {
            return Enum
                .GetValues<MealType>()
                .Select(m =>
                    new DropdownViewModel
                    {
                        Id = (int)m,
                        Name = m.ToString()
                    })
                .ToList();
        }


        public async Task<MealServiceStatusResponseModel> AddMealAsync(  CreateMealRequestData requestData, string userId)
        {
            MealServiceStatusResponseModel response =
                new MealServiceStatusResponseModel
                {
                    Status = false,
                    ErrorMessages = Enumerable.Empty<string?>()
                };

            try
            {
                if (!modelValidator.IsValid(requestData))
                {
                    response.ErrorMessages =
                        modelValidator
                            .ValidationResults
                            .Select(vr =>
                                vr.ErrorMessage)
                            .ToArray();

                    return response;
                }


                if (!IsMealTypeValid(requestData.MealType))
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Invalid meal type."
                        };

                    return response;
                }


                bool dishExists =
                    await DishExistsAsync(
                        requestData.DishId,
                        userId);

                if (!dishExists)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Dish does not exist."
                        };

                    return response;
                }


                DateTime selectedDate = requestData.Date!.Value.Date;


                (DailyMenu dailyMenu, _) =
                    await GetOrCreateDailyMenuAsync(
                        selectedDate,
                        userId);


                Meal meal = new Meal
                {
                    DishId =
                        requestData.DishId,

                    MealType =
                        requestData.MealType,

                    DailyMenuId =
                        dailyMenu.Id
                };


                mealRepository.AddAsync(meal);

                await mealRepository
                    .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while adding meal for user {UserId}.",
                    userId);

                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        ex.Message
                    };
            }


            return response;
        }


        public async Task<GetMealForEditResponseModel?> GetMealForEditAsync(  int id,  string userId)
        {
            GetMealForEditResponseModel? meal =
                await mealRepository
                    .GetAllAsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu
                            .WeeklyMenu
                            .UserId == userId)
                    .Select(m =>
                        new GetMealForEditResponseModel
                        {
                            Id = m.Id,
                            DishId = m.DishId,
                            Date = m.DailyMenu.Date,
                            MealType = m.MealType
                        })
                    .SingleOrDefaultAsync();


            return meal;
        }


        public bool IsMealTypeValid( MealType mealType)
        {
            return Enum.IsDefined(
                typeof(MealType),
                mealType);
        }


        public async Task<bool> DishExistsAsync(  int dishId, string userId)
        {
            return await dishRepository
                .GetAllAsNoTracking()
                .AnyAsync(d =>
                    d.Id == dishId &&
                    (
                        d.IsShared ||
                        d.UserId == userId
                    ));
        }


        public async Task<MealServiceStatusResponseModel> EditMealAsync( int id, EditMealRequestData requestData,string userId)
        {
            MealServiceStatusResponseModel response =
                new MealServiceStatusResponseModel
                {
                    Status = false,
                    ErrorMessages = Enumerable.Empty<string?>()
                };

            try
            {
                if (!modelValidator.IsValid(requestData))
                {
                    response.ErrorMessages =
                        modelValidator
                            .ValidationResults
                            .Select(vr =>
                                vr.ErrorMessage)
                            .ToArray();

                    return response;
                }


                if (!IsMealTypeValid(
                        requestData.MealType))
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Invalid meal type."
                        };

                    return response;
                }


                bool dishExists =
                    await DishExistsAsync(
                        requestData.DishId,
                        userId);

                if (!dishExists)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Dish does not exist."
                        };

                    return response;
                }


                Meal? mealToEdit =
                    await mealRepository
                        .GetAll()
                        .FirstOrDefaultAsync(m =>
                            m.Id == id &&
                            m.DailyMenu
                                .WeeklyMenu
                                .UserId == userId);


                if (mealToEdit == null)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Meal was not found."
                        };

                    return response;
                }


                DateTime selectedDate = requestData.Date!.Value.Date;


                (DailyMenu dailyMenu, _) =
                    await GetOrCreateDailyMenuAsync(
                        selectedDate,
                        userId);


                mealToEdit.DishId =
                    requestData.DishId;

                mealToEdit.MealType =
                    requestData.MealType;

                mealToEdit.DailyMenuId =
                    dailyMenu.Id;


                await mealRepository
                    .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while editing meal {MealId} for user {UserId}.",
                    id,
                    userId);

                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        ex.Message
                    };
            }


            return response;
        }


        public async Task<GetMealByIdResponseModel?> GetMealForDeleteAsync( int id, string userId)
        {
            GetMealByIdResponseModel? meal =
                await mealRepository
                    .GetAllAsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu
                            .WeeklyMenu
                            .UserId == userId)
                    .Select(m =>
                        new GetMealByIdResponseModel
                        {
                            Id = m.Id,

                            DishId = m.DishId,

                            DishName =
                                m.Dish.Name,

                            Date =
                                m.DailyMenu.Date,

                            MealType =
                                m.MealType,

                            Calories =
                                m.Dish.Calories,

                            PreparationTime =
                                m.Dish.PreparationTime,

                            Portions =
                                m.Dish.Portions,

                            ImageUrl =
                                m.Dish.ImageUrl
                        })
                    .SingleOrDefaultAsync();


            return meal;
        }


        public async Task<MealServiceStatusResponseModel> DeleteMealAsync( int id, string userId)
        {
            MealServiceStatusResponseModel response =
                new MealServiceStatusResponseModel
                {
                    Status = false,
                    ErrorMessages =Enumerable.Empty<string?>()
                };

            try
            {
                Meal? mealToDelete =
                    await mealRepository
                        .GetAll()
                        .FirstOrDefaultAsync(m =>
                            m.Id == id &&
                            m.DailyMenu
                                .WeeklyMenu
                                .UserId == userId);


                if (mealToDelete == null)
                {
                    response.ErrorMessages =
                        new[]
                        {
                            "Meal was not found."
                        };

                    return response;
                }


                mealRepository.Remove(mealToDelete);


                await mealRepository .SaveChangesAsync();


                response.Status = true;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error while deleting meal {MealId} for user {UserId}.",
                    id,
                    userId);

                response.Status = false;

                response.ErrorMessages =
                    new[]
                    {
                        ex.Message
                    };
            }


            return response;
        }


        public async Task<GetMealDetailsResponseModel?>  GetMealDetailsAsync( int id, string userId)
        {
            GetMealDetailsResponseModel? meal =
                await mealRepository
                    .GetAllAsNoTracking()
                    .Where(m =>
                        m.Id == id &&
                        m.DailyMenu
                            .WeeklyMenu
                            .UserId == userId)
                    .Select(m =>
                        new GetMealDetailsResponseModel
                        {
                            Id = m.Id,

                            DishId = m.DishId,

                            Date =
                                m.DailyMenu.Date,

                            DishName =
                                m.Dish.Name,

                            Description =
                                m.Dish.Description,

                            DishType =  m.Dish.DishType,

                            MealType = m.MealType,

                            Calories =
                                m.Dish.Calories,

                            PreparationTime =
                                m.Dish.PreparationTime,

                            Portions =
                                m.Dish.Portions,

                            ImageUrl =
                                m.Dish.ImageUrl
                        })
                    .SingleOrDefaultAsync();


            return meal;
        }


        private async Task<(DailyMenu DailyMenu, bool WasCreated)>GetOrCreateDailyMenuAsync( DateTime selectedDate, string userId)
        {
            DateTime weekStartDate = GetWeekStart(selectedDate);


            WeeklyMenu? weeklyMenu =
                await weeklyMenuRepository
                    .GetAll()
                    .FirstOrDefaultAsync(w =>
                        w.UserId == userId &&
                        w.WeekStartDate.Date ==
                        weekStartDate);


            if (weeklyMenu == null)
            {
                weeklyMenu =
                    new WeeklyMenu
                    {
                        WeekStartDate =
                            weekStartDate,

                        Title =
                            $"Week of {weekStartDate:dd.MM.yyyy}",

                        UserId =
                            userId
                    };


                weeklyMenuRepository.AddAsync(weeklyMenu);


                await weeklyMenuRepository
                    .SaveChangesAsync();
            }


            DailyMenu? dailyMenu =
                await dailyMenuRepository
                    .GetAll()
                    .FirstOrDefaultAsync(d =>
                        d.WeeklyMenu.UserId ==
                        userId &&
                        d.Date.Date ==
                        selectedDate);


            if (dailyMenu != null)
            {
                return (
                    dailyMenu,
                    false);
            }


            dailyMenu =
                new DailyMenu
                {
                    Date =
                        selectedDate,

                    WeeklyMenuId =
                        weeklyMenu.Id
                };


            dailyMenuRepository
                .AddAsync(dailyMenu);


            await dailyMenuRepository
                .SaveChangesAsync();


            return (
                dailyMenu,
                true);
        }


        private static DateTime GetWeekStart( DateTime date)
        {
            int difference =
                (
                    (int)date.DayOfWeek -
                    (int)DayOfWeek.Monday +
                    7
                ) % 7;


            return date
                .AddDays(-difference)
                .Date;
        }
    }
}
namespace NutriWeek.Services.Tests
{
    using Microsoft.Extensions.Logging;
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using GCommon.Utilities;
    using Services.Core.Data.Common.Response;
    using Services.Core.Data.Meal.Response;
    using Services.Core.Implementations;
    using Services.Core.Interfaces;

    [TestFixture]
    public class MealServiceTests
    {
        private Mock<IMealRepository> mealRepositoryMock = null!;
        private Mock<IDishRepository> dishRepositoryMock = null!;
        private Mock<IDailyMenuRepository> dailyMenuRepositoryMock = null!;
        private Mock<IWeeklyMenuRepository> weeklyMenuRepositoryMock = null!;
        private Mock<ILogger<MealService>> loggerMock = null!;
        private BasicModelValidator modelValidator = null!;
        private IMealService mealService = null!;

        [SetUp]
        public void SetUp()
        {
            this.mealRepositoryMock = new Mock<IMealRepository>();
            this.dishRepositoryMock = new Mock<IDishRepository>();
            this.dailyMenuRepositoryMock = new Mock<IDailyMenuRepository>();
            this.weeklyMenuRepositoryMock = new Mock<IWeeklyMenuRepository>();
            this.loggerMock = new Mock<ILogger<MealService>>();
            this.modelValidator = new BasicModelValidator();
            this.mealService = new MealService(
                this.mealRepositoryMock.Object,
                this.dishRepositoryMock.Object,
                this.dailyMenuRepositoryMock.Object,
                this.weeklyMenuRepositoryMock.Object,
                this.modelValidator,
                this.loggerMock.Object);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-GETALL-01")]
        public async Task GetAllAsync_ReturnsOnlyCurrentUsersMealsOrderedByDateAndMealType_Positive()
        {
            List<Meal> mockMeals = new List<Meal>()
            {
                CreateMeal(1, "user-1", new DateTime(2026, 11, 27), MealType.Dinner, "Dinner Dish", 500),
                CreateMeal(2, "user-1", new DateTime(2026, 11, 26), MealType.Lunch, "Lunch Dish", 400),
                CreateMeal(3, "user-1", new DateTime(2026, 11, 26), MealType.Breakfast, "Breakfast Dish", 300),
                CreateMeal(4, "user-2", new DateTime(2026, 11, 26), MealType.Breakfast, "Other User Dish", 250)
            };

            this.mealRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockMeals.BuildMock());

            List<GetAllMealsResponseModel> actualResult =
                await this.mealService.GetAllAsync("user-1");

            Assert.That(actualResult.Count, Is.EqualTo(3));
            Assert.Multiple(() =>
            {
                Assert.That(actualResult[0].Id, Is.EqualTo(3));
                Assert.That(actualResult[1].Id, Is.EqualTo(2));
                Assert.That(actualResult[2].Id, Is.EqualTo(1));
                Assert.That(actualResult.All(m => m.DishName != "Other User Dish"), Is.True);
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-GETALL-02")]
        public async Task GetAllAsync_NoMealsForUser_ReturnsEmptyCollection_Positive()
        {
            this.mealRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Meal>().BuildMock());

            List<GetAllMealsResponseModel> actualResult =
                await this.mealService.GetAllAsync("user-1");

            Assert.NotNull(actualResult);
            Assert.Zero(actualResult.Count);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DISHES-01")]
        public async Task LoadAvailableDishesAsync_ReturnsOwnAndSharedNonDeletedDishesOrderedByName_Positive()
        {
            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Zucchini Bowl", "user-1", false, false),
                CreateDish(2, "Apple Salad", null, true, false),
                CreateDish(3, "Other User Dish", "user-2", false, false),
                CreateDish(4, "Deleted Dish", "user-1", false, true)
            };

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            IEnumerable<DropdownResponseModel> actualResult =
                await this.mealService.LoadAvailableDishesAsync("user-1");

            List<DropdownResponseModel> result = actualResult.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(result.Count, Is.EqualTo(2));
                Assert.That(result[0].Name, Is.EqualTo("Apple Salad"));
                Assert.That(result[1].Name, Is.EqualTo("Zucchini Bowl"));
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-TYPES-01")]
        public void LoadMealTypes_ReturnsAllMealTypes_Positive()
        {
            List<DropdownResponseModel> actualResult =
                this.mealService.LoadMealTypes().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Count, Is.EqualTo(Enum.GetValues<MealType>().Length));
                Assert.That(actualResult.Any(m => m.Id == (int)MealType.Breakfast && m.Name == "Breakfast"), Is.True);
                Assert.That(actualResult.Any(m => m.Id == (int)MealType.Lunch && m.Name == "Lunch"), Is.True);
                Assert.That(actualResult.Any(m => m.Id == (int)MealType.Dinner && m.Name == "Dinner"), Is.True);
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-TYPES-02")]
        public void IsMealTypeValid_ReturnsCorrectResult_Positive()
        {
            bool validResult = this.mealService.IsMealTypeValid(MealType.Lunch);
            bool invalidResult = this.mealService.IsMealTypeValid((MealType)999);

            Assert.Multiple(() =>
            {
                Assert.That(validResult, Is.True);
                Assert.That(invalidResult, Is.False);
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DISHEXISTS-01")]
        public async Task DishExistsAsync_ReturnsTrueOnlyForVisibleNonDeletedDish_Positive()
        {
            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Own Dish", "user-1", false, false),
                CreateDish(2, "Shared Dish", null, true, false),
                CreateDish(3, "Other User Dish", "user-2", false, false),
                CreateDish(4, "Deleted Dish", "user-1", false, true)
            };

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            bool ownDish = await this.mealService.DishExistsAsync(1, "user-1");
            bool sharedDish = await this.mealService.DishExistsAsync(2, "user-1");
            bool otherDish = await this.mealService.DishExistsAsync(3, "user-1");
            bool deletedDish = await this.mealService.DishExistsAsync(4, "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(ownDish, Is.True);
                Assert.That(sharedDish, Is.True);
                Assert.That(otherDish, Is.False);
                Assert.That(deletedDish, Is.False);
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-EDIT-LOAD-01")]
        public async Task GetMealForEditAsync_OwnedMeal_ReturnsMappedMeal_Positive()
        {
            Meal meal = CreateMeal(
                1,
                "user-1",
                new DateTime(2026, 11, 26),
                MealType.Lunch,
                "Chicken Rice",
                500);

            this.mealRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Meal>() { meal }.BuildMock());

            GetMealForEditResponseModel? actualResult =
                await this.mealService.GetMealForEditAsync(1, "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult!.Id, Is.EqualTo(meal.Id));
                Assert.That(actualResult.DishId, Is.EqualTo(meal.DishId));
                Assert.That(actualResult.Date, Is.EqualTo(meal.DailyMenu.Date));
                Assert.That(actualResult.MealType, Is.EqualTo(meal.MealType));
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DELETE-LOAD-01")]
        public async Task GetMealForDeleteAsync_OwnedMeal_ReturnsMappedMeal_Positive()
        {
            Meal meal = CreateMeal(
                1,
                "user-1",
                new DateTime(2026, 11, 26),
                MealType.Dinner,
                "Fish",
                450);

            this.mealRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Meal>() { meal }.BuildMock());

            GetMealByIdResponseModel? actualResult =
                await this.mealService.GetMealForDeleteAsync(1, "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult!.Id, Is.EqualTo(meal.Id));
                Assert.That(actualResult.DishId, Is.EqualTo(meal.DishId));
                Assert.That(actualResult.DishName, Is.EqualTo(meal.Dish.Name));
                Assert.That(actualResult.Date, Is.EqualTo(meal.DailyMenu.Date));
                Assert.That(actualResult.MealType, Is.EqualTo(meal.MealType));
                Assert.That(actualResult.Calories, Is.EqualTo(meal.Dish.Calories));
            });
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DETAILS-01")]
        public async Task GetMealDetailsAsync_OwnedMeal_ReturnsMappedDetails_Positive()
        {
            Meal meal = CreateMeal(
                1,
                "user-1",
                new DateTime(2026, 11, 26),
                MealType.Breakfast,
                "Oatmeal",
                350);

            this.mealRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Meal>() { meal }.BuildMock());

            GetMealDetailsResponseModel? actualResult =
                await this.mealService.GetMealDetailsAsync(1, "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult!.Id, Is.EqualTo(meal.Id));
                Assert.That(actualResult.DishId, Is.EqualTo(meal.DishId));
                Assert.That(actualResult.DishName, Is.EqualTo(meal.Dish.Name));
                Assert.That(actualResult.Date, Is.EqualTo(meal.DailyMenu.Date));
                Assert.That(actualResult.MealType, Is.EqualTo(meal.MealType));
                Assert.That(actualResult.Calories, Is.EqualTo(meal.Dish.Calories));
            });
        }

        private static Meal CreateMeal(
            int id,
            string userId,
            DateTime date,
            MealType mealType,
            string dishName,
            int calories)
        {
            WeeklyMenu weeklyMenu = new WeeklyMenu()
            {
                Id = 1,
                UserId = userId,
                WeekStartDate = date.AddDays(-(((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7)),
                Title = "Test Week"
            };

            DailyMenu dailyMenu = new DailyMenu()
            {
                Id = id,
                Date = date,
                WeeklyMenuId = weeklyMenu.Id,
                WeeklyMenu = weeklyMenu
            };

            Dish dish = new Dish()
            {
                Id = id,
                Name = dishName,
                Description = "Valid description",
                Ingredients = "Valid ingredients",
                Instructions = "Valid instructions",
                Calories = calories,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = userId,
                IsShared = false,
                IsDeleted = false
            };

            return new Meal()
            {
                Id = id,
                MealType = mealType,
                DishId = dish.Id,
                Dish = dish,
                DailyMenuId = dailyMenu.Id,
                DailyMenu = dailyMenu
            };
        }

        private static Dish CreateDish(
            int id,
            string name,
            string? userId,
            bool isShared,
            bool isDeleted)
        {
            return new Dish()
            {
                Id = id,
                Name = name,
                Description = "Valid description",
                Ingredients = "Valid ingredients",
                Instructions = "Valid instructions",
                Calories = 400,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = userId,
                IsShared = isShared,
                IsDeleted = isDeleted
            };
        }
    }
}

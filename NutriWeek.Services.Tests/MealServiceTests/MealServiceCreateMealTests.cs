namespace NutriWeek.Services.Tests
{
    using Microsoft.Extensions.Logging;
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using GCommon.Utilities;
    using Services.Core.Data.Meal.Request;
    using Services.Core.Implementations;
    using Services.Core.Interfaces;

    [TestFixture]
    public class MealServiceCreateMealTests
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
            this.mealRepositoryMock = new Mock<IMealRepository>(MockBehavior.Strict);
            this.dishRepositoryMock = new Mock<IDishRepository>(MockBehavior.Strict);
            this.dailyMenuRepositoryMock = new Mock<IDailyMenuRepository>(MockBehavior.Strict);
            this.weeklyMenuRepositoryMock = new Mock<IWeeklyMenuRepository>(MockBehavior.Strict);
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
        [Property("Requirement", "REQ-MLS-CREATE-01")]
        public async Task AddMealAsync_InvalidRequest_ReturnsErrorsAndDoesNotPersist_Negative()
        {
            CreateMealRequestData request = ValidRequest();
            request.DishId = 0;

            var result = await this.mealService.AddMealAsync(request, "user-1");

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.Not.Empty);
            this.mealRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meal>()), Times.Never);
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-CREATE-02")]
        public async Task AddMealAsync_InvalidMealType_ReturnsErrorAndDoesNotPersist_Negative()
        {
            CreateMealRequestData request = ValidRequest();
            request.MealType = (MealType)999;

            var result = await this.mealService.AddMealAsync(request, "user-1");

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Invalid meal type." }));
            this.mealRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meal>()), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-CREATE-03")]
        public async Task AddMealAsync_DishDoesNotExist_ReturnsErrorAndDoesNotPersist_Negative()
        {
            CreateMealRequestData request = ValidRequest();

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>().BuildMock());

            var result = await this.mealService.AddMealAsync(request, "user-1");

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Dish does not exist." }));
            this.mealRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meal>()), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-CREATE-04")]
        public async Task AddMealAsync_ExistingDailyMenu_CreatesMealAndReturnsSuccess_Positive()
        {
            string userId = "user-1";
            CreateMealRequestData request = ValidRequest();
            Dish dish = CreateDish(request.DishId, userId);
            WeeklyMenu weeklyMenu = CreateWeeklyMenu(10, userId, request.Date!.Value);
            DailyMenu dailyMenu = CreateDailyMenu(20, request.Date.Value.Date, weeklyMenu);
            Meal? capturedMeal = null;

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());
            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());
            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<DailyMenu>() { dailyMenu }.BuildMock());
            this.mealRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Meal>()))
                .Callback<Meal>(m => capturedMeal = m)
                .Returns(Task.CompletedTask);
            this.mealRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.mealService.AddMealAsync(request, userId);

            Assert.That(result.Status, Is.True);
            Assert.That(capturedMeal, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(capturedMeal!.DishId, Is.EqualTo(request.DishId));
                Assert.That(capturedMeal.MealType, Is.EqualTo(request.MealType));
                Assert.That(capturedMeal.DailyMenuId, Is.EqualTo(dailyMenu.Id));
            });
            this.mealRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Meal>()), Times.Once);
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-CREATE-05")]
        public async Task AddMealAsync_NoMenus_CreatesWeeklyDailyAndMeal_Positive()
        {
            string userId = "user-1";
            CreateMealRequestData request = ValidRequest();
            Dish dish = CreateDish(request.DishId, userId);
            WeeklyMenu? capturedWeeklyMenu = null;
            DailyMenu? capturedDailyMenu = null;
            Meal? capturedMeal = null;

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());
            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<WeeklyMenu>().BuildMock());
            this.weeklyMenuRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<WeeklyMenu>()))
                .Callback<WeeklyMenu>(w =>
                {
                    w.Id = 10;
                    capturedWeeklyMenu = w;
                })
                .Returns(Task.CompletedTask);
            this.weeklyMenuRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<DailyMenu>().BuildMock());
            this.dailyMenuRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DailyMenu>()))
                .Callback<DailyMenu>(d =>
                {
                    d.Id = 20;
                    capturedDailyMenu = d;
                })
                .Returns(Task.CompletedTask);
            this.dailyMenuRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            this.mealRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Meal>()))
                .Callback<Meal>(m => capturedMeal = m)
                .Returns(Task.CompletedTask);
            this.mealRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.mealService.AddMealAsync(request, userId);

            Assert.That(result.Status, Is.True);
            Assert.That(capturedWeeklyMenu, Is.Not.Null);
            Assert.That(capturedDailyMenu, Is.Not.Null);
            Assert.That(capturedMeal, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(capturedWeeklyMenu!.UserId, Is.EqualTo(userId));
                Assert.That(capturedDailyMenu!.WeeklyMenuId, Is.EqualTo(10));
                Assert.That(capturedMeal!.DailyMenuId, Is.EqualTo(20));
            });
        }

        private static CreateMealRequestData ValidRequest() => new CreateMealRequestData()
        {
            DishId = 1,
            Date = new DateTime(2026, 11, 26),
            MealType = MealType.Lunch
        };

        private static Dish CreateDish(int id, string userId)
        {
            return new Dish()
            {
                Id = id,
                Name = "Chicken Rice",
                Description = "Valid description",
                Ingredients = "Valid ingredients",
                Instructions = "Valid instructions",
                Calories = 500,
                PreparationTime = 30,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = userId,
                IsShared = false,
                IsDeleted = false
            };
        }

        private static WeeklyMenu CreateWeeklyMenu(int id, string userId, DateTime date)
        {
            DateTime weekStart = date
                .AddDays(-(((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7))
                .Date;

            return new WeeklyMenu()
            {
                Id = id,
                UserId = userId,
                WeekStartDate = weekStart,
                Title = $"Week of {weekStart:dd.MM.yyyy}"
            };
        }

        private static DailyMenu CreateDailyMenu(int id, DateTime date, WeeklyMenu weeklyMenu)
        {
            return new DailyMenu()
            {
                Id = id,
                Date = date,
                WeeklyMenuId = weeklyMenu.Id,
                WeeklyMenu = weeklyMenu
            };
        }
    }
}

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
    public class MealServiceEditDeleteTests
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
        [Property("Requirement", "REQ-MLS-EDIT-01")]
        public async Task EditMealAsync_InvalidRequest_ReturnsErrorsAndDoesNotPersist_Negative()
        {
            EditMealRequestData request = ValidEditRequest();
            request.DishId = 0;

            var result = await this.mealService.EditMealAsync(1, request, "user-1");

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.Not.Empty);
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-EDIT-02")]
        public async Task EditMealAsync_MealNotFound_ReturnsError_Negative()
        {
            string userId = "user-1";
            EditMealRequestData request = ValidEditRequest();
            Dish dish = CreateDish(request.DishId, userId);

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());
            this.mealRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Meal>().BuildMock());

            var result = await this.mealService.EditMealAsync(1, request, userId);

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Meal was not found." }));
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-EDIT-03")]
        public async Task EditMealAsync_ValidRequest_UpdatesMealAndReturnsSuccess_Positive()
        {
            string userId = "user-1";
            EditMealRequestData request = ValidEditRequest();
            Dish dish = CreateDish(request.DishId, userId);
            WeeklyMenu weeklyMenu = CreateWeeklyMenu(10, userId, request.Date!.Value);
            DailyMenu currentDailyMenu = CreateDailyMenu(20, new DateTime(2026, 11, 25), weeklyMenu);
            DailyMenu selectedDailyMenu = CreateDailyMenu(21, request.Date.Value.Date, weeklyMenu);
            Meal meal = new Meal()
            {
                Id = 1,
                DishId = 2,
                Dish = CreateDish(2, userId),
                MealType = MealType.Breakfast,
                DailyMenuId = currentDailyMenu.Id,
                DailyMenu = currentDailyMenu
            };

            this.dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());
            this.mealRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Meal>() { meal }.BuildMock());
            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());
            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<DailyMenu>() { selectedDailyMenu }.BuildMock());
            this.mealRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.mealService.EditMealAsync(1, request, userId);

            Assert.That(result.Status, Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(meal.DishId, Is.EqualTo(request.DishId));
                Assert.That(meal.MealType, Is.EqualTo(request.MealType));
                Assert.That(meal.DailyMenuId, Is.EqualTo(selectedDailyMenu.Id));
            });
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DELETE-01")]
        public async Task DeleteMealAsync_OwnedMeal_RemovesMealAndReturnsSuccess_Positive()
        {
            string userId = "user-1";
            WeeklyMenu weeklyMenu = CreateWeeklyMenu(10, userId, new DateTime(2026, 11, 26));
            DailyMenu dailyMenu = CreateDailyMenu(20, new DateTime(2026, 11, 26), weeklyMenu);
            Meal meal = new Meal()
            {
                Id = 1,
                DishId = 1,
                Dish = CreateDish(1, userId),
                MealType = MealType.Lunch,
                DailyMenuId = dailyMenu.Id,
                DailyMenu = dailyMenu
            };

            this.mealRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Meal>() { meal }.BuildMock());
            this.mealRepositoryMock
                .Setup(r => r.Remove(meal));
            this.mealRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.mealService.DeleteMealAsync(1, userId);

            Assert.That(result.Status, Is.True);
            this.mealRepositoryMock.Verify(r => r.Remove(meal), Times.Once);
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-MLS-DELETE-02")]
        public async Task DeleteMealAsync_NotOwnedOrMissingMeal_ReturnsError_Negative()
        {
            this.mealRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Meal>().BuildMock());

            var result = await this.mealService.DeleteMealAsync(1, "user-1");

            Assert.That(result.Status, Is.False);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Meal was not found." }));
            this.mealRepositoryMock.Verify(r => r.Remove(It.IsAny<Meal>()), Times.Never);
            this.mealRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        private static EditMealRequestData ValidEditRequest() => new EditMealRequestData()
        {
            DishId = 1,
            Date = new DateTime(2026, 11, 26),
            MealType = MealType.Dinner
        };

        private static Dish CreateDish(int id, string userId)
        {
            return new Dish()
            {
                Id = id,
                Name = $"Dish {id}",
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

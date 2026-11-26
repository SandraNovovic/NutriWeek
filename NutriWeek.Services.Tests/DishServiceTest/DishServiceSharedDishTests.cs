namespace NutriWeek.Services.Tests.DishServiceTest
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using GCommon.Utilities;
    using Services.Core.Data.Dish.Response;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class DishServiceSharedDishTests
    {
        private Mock<IDishRepository> dishRepositoryMock = null!;
        private Mock<IIngredientRepository> ingredientRepositoryMock = null!;
        private Mock<IDishIngredientRepository> dishIngredientRepositoryMock = null!;
        private Mock<ILogger<DishService>> loggerMock = null!;
        private BasicModelValidator modelValidator = null!;
        private IDishService dishService = null!;

        [SetUp]
        public void SetUp()
        {
            dishRepositoryMock = new Mock<IDishRepository>(MockBehavior.Strict);
            ingredientRepositoryMock = new Mock<IIngredientRepository>(MockBehavior.Strict);
            dishIngredientRepositoryMock = new Mock<IDishIngredientRepository>(MockBehavior.Strict);
            loggerMock = new Mock<ILogger<DishService>>();
            modelValidator = new BasicModelValidator();
            dishService = new DishService(
                dishRepositoryMock.Object,
                modelValidator,
                loggerMock.Object,
                ingredientRepositoryMock.Object,
                dishIngredientRepositoryMock.Object);
        }

        [Test]
        [Property("Requirement", "REQ-DS-SHARED-01")]
        public async Task GetSharedDishesAsync_ReturnsOnlySharedDishesOrderedByDeletedThenName_Positive()
        {
            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Zebra", true, false),
                CreateDish(2, "Alpha", true, false),
                CreateDish(3, "Deleted", true, true),
                CreateDish(4, "Private", false, false)
            };

            dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            IEnumerable<GetAllDishesResponseModel> result =
                await dishService.GetSharedDishesAsync();
            List<GetAllDishesResponseModel> resultList = result.ToList();

            Assert.That(resultList.Count, Is.EqualTo(3));
            Assert.That(resultList[0].Name, Is.EqualTo("Alpha"));
            Assert.That(resultList[1].Name, Is.EqualTo("Zebra"));
            Assert.That(resultList[2].Name, Is.EqualTo("Deleted"));
            Assert.IsTrue(resultList.All(d => d.IsShared));
        }



        [TestCase(true)]
        [Property("Requirement", "REQ-DS-SHARED-STATUS-01")]
        public async Task SetSharedDishDeletedStatusAsync_SharedDish_UpdatesStatusAndReturnsSuccess_Positive(
            bool isDeleted)
        {
            Dish dish = CreateDish(1, "Shared Dish", true, !isDeleted);

            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>() { dish }.BuildMock());
            dishRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await dishService.SetSharedDishDeletedStatusAsync(1, isDeleted);

            Assert.IsTrue(result.Status);
            Assert.That(dish.IsDeleted, Is.EqualTo(isDeleted));
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-DS-SHARED-STATUS-02")]
        public async Task SetSharedDishDeletedStatusAsync_NotFoundOrPrivateDish_ReturnsNotFound_Negative()
        {
            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>()
                {
                    CreateDish(1, "Private Dish", false, false)
                }.BuildMock());

            var result = await dishService.SetSharedDishDeletedStatusAsync(1, true);

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Shared dish not found." }));
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }



        private static Dish CreateDish(int id, string name, bool isShared, bool isDeleted)
        {
            return new Dish()
            {
                Id = id,
                Name = name,
                Description = "Valid dish description",
                Ingredients = "Valid ingredients",
                Instructions = "Valid cooking instructions",
                Calories = 300,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = isShared ? null : "user-1",
                IsShared = isShared,
                IsDeleted = isDeleted
            };
        }
    }
}

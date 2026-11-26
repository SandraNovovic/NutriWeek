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
    public class DishServiceDeleteDishTests
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
        [Property("Requirement", "REQ-DS-DELETE-LOAD-01")]
        public async Task GetDishForDeleteAsync_OwnedDish_ReturnsMappedDish_Positive()
        {
            Dish dish = CreateDish(1, "user-1", false);

            dishRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());

            GetDishByIdResponseModel? result =
                await dishService.GetDishForDeleteAsync(1, "user-1");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(dish.Id));
            Assert.That(result.Name, Is.EqualTo(dish.Name));
            Assert.That(result.DishType, Is.EqualTo(dish.DishType));
        }



        [Test]
        [Property("Requirement", "REQ-DS-DELETE-01")]
        public async Task DeleteDishAsync_OwnedDish_SoftDeletesAndReturnsSuccess_Positive()
        {
            Dish dish = CreateDish(1, "user-1", false);

            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>() { dish }.BuildMock());
            dishRepositoryMock
                .Setup(r => r.SoftDelete(dish))
                .Callback<Dish>(d => d.IsDeleted = true);
            dishRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await dishService.DeleteDishAsync(1, "user-1");

            Assert.IsTrue(result.Status);
            Assert.IsTrue(dish.IsDeleted);
            dishRepositoryMock.Verify(r => r.SoftDelete(dish), Times.Once);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-DS-DELETE-02")]
        public async Task DeleteDishAsync_DishNotFoundOrNotOwned_ReturnsNotFound_Negative()
        {
            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>().BuildMock());

            var result = await dishService.DeleteDishAsync(1, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Dish not found." }));
            dishRepositoryMock.Verify(r => r.SoftDelete(It.IsAny<Dish>()), Times.Never);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }



        private static Dish CreateDish(int id, string userId, bool isDeleted)
        {
            return new Dish()
            {
                Id = id,
                Name = $"Dish {id}",
                Description = "Valid dish description",
                Ingredients = "Valid ingredients",
                Instructions = "Valid cooking instructions",
                Calories = 300,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = userId,
                IsShared = false,
                IsDeleted = isDeleted
            };
        }
    }
}

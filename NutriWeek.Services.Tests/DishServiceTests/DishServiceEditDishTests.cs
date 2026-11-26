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
    using Services.Core.Data.Dish.Request;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class DishServiceEditDishTests
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

        private static EditDishRequestData ValidRequest() => new EditDishRequestData()
        {
            Name = "Updated Chicken Rice",
            Description = "Updated healthy chicken rice dish",
            Ingredients = "Chicken, rice, vegetables",
            Instructions = "Cook all ingredients and serve hot.",
            Calories = 450,
            PreparationTime = 25,
            Portions = 3,
            ImageUrl = "https://example.com/updated.jpg",
            DishType = DishType.Meat,
            IngredientItems = new List<DishIngredientRequestData>()
        };

        [Test]
        [Property("Requirement", "REQ-DS-EDIT-01")]
        public async Task EditDishAsync_InvalidRequest_ReturnsErrorsAndDoesNotPersist_Negative()
        {
            EditDishRequestData request = ValidRequest();
            request.Name = "A";

            var result = await dishService.EditDishAsync(1, request, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.Not.Empty);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }


        [Test]
        [Property("Requirement", "REQ-DS-EDIT-03")]
        public async Task EditDishAsync_DuplicateIngredients_ReturnsErrorAndDoesNotPersist_Negative()
        {
            EditDishRequestData request = ValidRequest();
            request.IngredientItems = new List<DishIngredientRequestData>()
            {
                new DishIngredientRequestData()
                {
                    Name = "Rice",
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                },
                new DishIngredientRequestData()
                {
                    Name = " rice ",
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            };

            var result = await dishService.EditDishAsync(1, request, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages,
                Is.EqualTo(new[] { "Each ingredient can only be added once to a dish." }));
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-DS-EDIT-04")]
        public async Task EditDishAsync_DishNotFoundOrNotOwned_ReturnsNotFound_Negative()
        {
            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>().BuildMock());

            var result = await dishService.EditDishAsync(1, ValidRequest(), "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Dish not found." }));
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-DS-EDIT-05")]
        [Property("Requirement", "REQ-DS-EDIT-06")]
        public async Task EditDishAsync_ValidRequest_UpdatesDishAndReplacesIngredients_Positive()
        {
            Dish dish = CreateOwnedDish();
            DishIngredient oldDishIngredient = new DishIngredient()
            {
                Id = 1,
                Dish = dish,
                DishId = dish.Id,
                Ingredient = new Ingredient() { Id = 1, Name = "Old Rice" },
                IngredientId = 1,
                Quantity = 50,
                Unit = MeasurementUnit.Gram
            };
            dish.DishIngredients.Add(oldDishIngredient);

            EditDishRequestData request = ValidRequest();
            request.IngredientItems = new List<DishIngredientRequestData>()
            {
                new DishIngredientRequestData()
                {
                    Name = "Chicken",
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            };

            dishRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Dish>() { dish }.BuildMock());
            dishIngredientRepositoryMock
                .Setup(r => r.Remove(oldDishIngredient));
            ingredientRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Ingredient>().BuildMock());
            ingredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ingredient>()))
                .Returns(Task.CompletedTask);
            dishIngredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DishIngredient>()))
                .Returns(Task.CompletedTask);
            dishRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await dishService.EditDishAsync(1, request, "user-1");

            Assert.IsTrue(result.Status);
            Assert.That(dish.Name, Is.EqualTo(request.Name));
            Assert.That(dish.Description, Is.EqualTo(request.Description));
            Assert.That(dish.Ingredients, Is.EqualTo(request.Ingredients));
            Assert.That(dish.Instructions, Is.EqualTo(request.Instructions));
            Assert.That(dish.Calories, Is.EqualTo(request.Calories));
            Assert.That(dish.PreparationTime, Is.EqualTo(request.PreparationTime));
            Assert.That(dish.Portions, Is.EqualTo(request.Portions));
            Assert.That(dish.ImageUrl, Is.EqualTo(request.ImageUrl));
            Assert.That(dish.DishType, Is.EqualTo(request.DishType));
            dishIngredientRepositoryMock.Verify(r => r.Remove(oldDishIngredient), Times.Once);
            ingredientRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Ingredient>(i => i.Name == "Chicken")),
                Times.Once);
            dishIngredientRepositoryMock.Verify(
                r => r.AddAsync(It.Is<DishIngredient>(di =>
                    di.Dish == dish &&
                    di.Quantity == 200 &&
                    di.Unit == MeasurementUnit.Gram)),
                Times.Once);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }



        private static Dish CreateOwnedDish()
        {
            return new Dish()
            {
                Id = 1,
                Name = "Old Dish",
                Description = "Old valid description",
                Ingredients = "Old ingredients",
                Instructions = "Old valid cooking instructions",
                Calories = 300,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                UserId = "user-1",
                IsShared = false,
                IsDeleted = false
            };
        }
    }
}

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
    public class DishServiceCreateDishTests
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

        private static CreateDishRequestData ValidRequest() => new CreateDishRequestData()
        {
            Name = "Chicken Rice",
            Description = "Healthy chicken rice dish",
            Ingredients = "Chicken, rice",
            Instructions = "Cook the chicken and rice together.",
            Calories = 500,
            PreparationTime = 30,
            Portions = 2,
            ImageUrl = "https://example.com/chicken-rice.jpg",
            DishType = DishType.Meat,
            IngredientItems = new List<DishIngredientRequestData>()
        };

        [Test]
        [Property("Requirement", "REQ-DS-CREATE-01")]
        public async Task AddDishAsync_InvalidRequest_ReturnsErrorsAndDoesNotPersist_Negative()
        {
            CreateDishRequestData request = ValidRequest();
            request.Name = "A";

            var result = await dishService.AddDishAsync(request, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.Not.Empty);
            dishRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Dish>()), Times.Never);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-DS-CREATE-02")]
        public async Task AddDishAsync_InvalidDishType_ReturnsErrorAndDoesNotPersist_Negative()
        {
            CreateDishRequestData request = ValidRequest();
            request.DishType = (DishType)999;

            var result = await dishService.AddDishAsync(request, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages, Is.EqualTo(new[] { "Invalid dish type." }));
            dishRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Dish>()), Times.Never);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-DS-CREATE-03")]
        public async Task AddDishAsync_DuplicateIngredients_ReturnsErrorAndDoesNotPersist_Negative()
        {
            CreateDishRequestData request = ValidRequest();
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

            var result = await dishService.AddDishAsync(request, "user-1");

            Assert.IsFalse(result.Status);
            Assert.That(result.ErrorMessages,
                Is.EqualTo(new[] { "Each ingredient can only be added once to a dish." }));
            dishRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Dish>()), Times.Never);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-DS-CREATE-04")]
        [Property("Requirement", "REQ-DS-CREATE-05")]
        public async Task AddDishAsync_ValidRequest_CreatesUserDishAndReturnsSuccess_Positive()
        {
            CreateDishRequestData request = ValidRequest();
            Dish? captured = null;

            dishRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Dish>()))
                .Callback<Dish>(d => captured = d)
                .Returns(Task.CompletedTask);
            dishRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await dishService.AddDishAsync(request, "user-1");

            Assert.IsTrue(result.Status);
            Assert.That(captured, Is.Not.Null);
            Assert.That(captured!.Name, Is.EqualTo(request.Name));
            Assert.That(captured.Description, Is.EqualTo(request.Description));
            Assert.That(captured.Ingredients, Is.EqualTo(request.Ingredients));
            Assert.That(captured.Instructions, Is.EqualTo(request.Instructions));
            Assert.That(captured.Calories, Is.EqualTo(request.Calories));
            Assert.That(captured.PreparationTime, Is.EqualTo(request.PreparationTime));
            Assert.That(captured.Portions, Is.EqualTo(request.Portions));
            Assert.That(captured.ImageUrl, Is.EqualTo(request.ImageUrl));
            Assert.That(captured.DishType, Is.EqualTo(request.DishType));
            Assert.That(captured.UserId, Is.EqualTo("user-1"));
            Assert.IsFalse(captured.IsShared);
            dishRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Dish>()), Times.Once);
            dishRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


        [Test]
        [Property("Requirement", "REQ-DS-CREATE-07")]
        [Property("Requirement", "REQ-DS-CREATE-08")]
        public async Task AddDishAsync_WithExistingAndNewIngredients_CreatesDishIngredients_Positive()
        {
            CreateDishRequestData request = ValidRequest();
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
                    Name = " Chicken ",
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                }
            };

            Ingredient rice = new Ingredient()
            {
                Id = 1,
                Name = "Rice"
            };
            IQueryable<Ingredient> ingredientsQueryableMock =
                new List<Ingredient>() { rice }.BuildMock();

            dishRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Dish>()))
                .Returns(Task.CompletedTask);
            dishRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);
            ingredientRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(ingredientsQueryableMock);
            ingredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Ingredient>()))
                .Returns(Task.CompletedTask);
            dishIngredientRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DishIngredient>()))
                .Returns(Task.CompletedTask);

            var result = await dishService.AddDishAsync(request, "user-1");

            Assert.IsTrue(result.Status);
            ingredientRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Ingredient>(i => i.Name == "Chicken")),
                Times.Once);
            dishIngredientRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DishIngredient>()),
                Times.Exactly(2));
            dishIngredientRepositoryMock.Verify(
                r => r.AddAsync(It.Is<DishIngredient>(di =>
                    di.Ingredient == rice &&
                    di.Quantity == 100 &&
                    di.Unit == MeasurementUnit.Gram)),
                Times.Once);
        }



    }
}

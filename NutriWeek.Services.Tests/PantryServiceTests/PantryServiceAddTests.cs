namespace NutriWeek.Services.Tests
{
    using Microsoft.Extensions.Logging;
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using GCommon.Utilities;
    using Services.Core.Data.Pantry.Request;
    using Services.Core.Data.Pantry.Response;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class PantryServiceAddTests
    {
        private Mock<IPantryItemRepository> pantryItemRepositoryMock = null!;
        private Mock<IIngredientRepository> ingredientRepositoryMock = null!;
        private Mock<ILogger<PantryService>> loggerMock = null!;
        private BasicModelValidator modelValidator = null!;
        private IPantryService pantryService = null!;

        [SetUp]
        public void SetUp()
        {
            this.pantryItemRepositoryMock =
                new Mock<IPantryItemRepository>(MockBehavior.Strict);

            this.ingredientRepositoryMock =
                new Mock<IIngredientRepository>(MockBehavior.Strict);

            this.loggerMock =
                new Mock<ILogger<PantryService>>();

            this.modelValidator =
                new BasicModelValidator();

            this.pantryService =
                new PantryService(
                    this.pantryItemRepositoryMock.Object,
                    this.ingredientRepositoryMock.Object,
                    this.modelValidator,
                    this.loggerMock.Object);
        }

        [Test]
        [Property("Requirement", "REQ-PS-ADD-01")]
        public async Task AddAsync_ValidRequest_AddsPantryItemAndReturnsSuccess_Positive()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            Ingredient ingredient =
                new Ingredient()
                {
                    Id = request.IngredientId,
                    Name = "Rice"
                };

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                    {
                        ingredient
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<PantryItem>()
                        .BuildMock());

            PantryItem? captured = null;

            this.pantryItemRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<PantryItem>()))
                .Callback<PantryItem>(p => captured = p)
                .Returns(Task.CompletedTask);

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddAsync(
                    request,
                    "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Status, Is.True);
                Assert.That(captured, Is.Not.Null);
                Assert.That(captured!.UserId, Is.EqualTo("user-1"));
                Assert.That(captured.IngredientId, Is.EqualTo(request.IngredientId));
                Assert.That(captured.Quantity, Is.EqualTo(request.Quantity));
                Assert.That(captured.Unit, Is.EqualTo(request.Unit));
            });

            this.pantryItemRepositoryMock
                .Verify(r => r.AddAsync(It.IsAny<PantryItem>()), Times.Once);

            this.pantryItemRepositoryMock
                .Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-PS-ADD-02")]
        public async Task AddAsync_InvalidRequest_ReturnsErrorsAndDoesNotPersist_Negative()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            request.Quantity = 0;

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddAsync(
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(actualResult.ErrorMessages, Is.Not.Empty);

            this.pantryItemRepositoryMock
                .Verify(r => r.AddAsync(It.IsAny<PantryItem>()), Times.Never);

            this.pantryItemRepositoryMock
                .Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Test]
        [Property("Requirement", "REQ-PS-ADD-03")]
        public async Task AddAsync_InvalidMeasurementUnit_ReturnsError_Negative()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            request.Unit = (MeasurementUnit)999;

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddAsync(
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain("Invalid measurement unit."));
        }

        [Test]
        [Property("Requirement", "REQ-PS-ADD-04")]
        public async Task AddAsync_IngredientDoesNotExist_ReturnsError_Negative()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                        .BuildMock());

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddAsync(
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain(
                    "Please select a valid ingredient from the suggestions."));
        }

        [Test]
        [Property("Requirement", "REQ-PS-ADD-05")]
        public async Task AddAsync_DuplicateIngredient_ReturnsError_Negative()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Id = request.IngredientId,
                            Name = "Rice"
                        }
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<PantryItem>()
                    {
                        new PantryItem()
                        {
                            Id = 1,
                            UserId = "user-1",
                            IngredientId = request.IngredientId,
                            Quantity = 100,
                            Unit = MeasurementUnit.Gram
                        }
                    }.BuildMock());

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddAsync(
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain(
                    "This ingredient is already in your pantry."));
        }

        [Test]
        [Property("Requirement", "REQ-PS-PURCHASED-01")]
        public async Task AddPurchasedItemAsync_ItemDoesNotExist_AddsNewPantryItem_Positive()
        {
            CreatePantryItemRequestData request =
                CreateValidRequest();

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Id = request.IngredientId,
                            Name = "Rice"
                        }
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                        .BuildMock());

            PantryItem? captured = null;

            this.pantryItemRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<PantryItem>()))
                .Callback<PantryItem>(p => captured = p)
                .Returns(Task.CompletedTask);

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddPurchasedItemAsync(
                    request,
                    "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Status, Is.True);
                Assert.That(captured, Is.Not.Null);
                Assert.That(captured!.IngredientId, Is.EqualTo(request.IngredientId));
                Assert.That(captured.Quantity, Is.EqualTo(request.Quantity));
            });
        }

        [Test]
        [Property("Requirement", "REQ-PS-PURCHASED-02")]
        public async Task AddPurchasedItemAsync_ExistingCompatibleItem_ConvertsAndAddsQuantity_Positive()
        {
            CreatePantryItemRequestData request =
                new CreatePantryItemRequestData()
                {
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                };

            PantryItem existingPantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Quantity = 0.5m,
                    Unit = MeasurementUnit.Kilogram
                };

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Id = 1,
                            Name = "Rice"
                        }
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        existingPantryItem
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddPurchasedItemAsync(
                    request,
                    "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Status, Is.True);
                Assert.That(existingPantryItem.Quantity, Is.EqualTo(1m));
            });
        }

        [Test]
        [Property("Requirement", "REQ-PS-PURCHASED-03")]
        public async Task AddPurchasedItemAsync_IncompatibleUnit_ReturnsError_Negative()
        {
            CreatePantryItemRequestData request =
                new CreatePantryItemRequestData()
                {
                    IngredientId = 1,
                    Quantity = 1,
                    Unit = MeasurementUnit.Piece
                };

            PantryItem existingPantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                };

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(
                    new List<Ingredient>()
                    {
                        new Ingredient()
                        {
                            Id = 1,
                            Name = "Rice"
                        }
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        existingPantryItem
                    }.BuildMock());

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.AddPurchasedItemAsync(
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain(
                    "The purchased quantity uses an incompatible measurement unit."));

            this.pantryItemRepositoryMock
                .Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        private static CreatePantryItemRequestData CreateValidRequest()
        {
            return new CreatePantryItemRequestData()
            {
                IngredientId = 1,
                Quantity = 500,
                Unit = MeasurementUnit.Gram
            };
        }
    }
}

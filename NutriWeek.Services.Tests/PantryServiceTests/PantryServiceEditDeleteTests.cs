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
    public class PantryServiceEditDeleteTests
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
        [Property("Requirement", "REQ-PS-EDIT-01")]
        public async Task EditAsync_ValidOwnedItem_UpdatesValuesAndReturnsSuccess_Positive()
        {
            PantryItem pantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        pantryItem
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            EditPantryItemRequestData request =
                new EditPantryItemRequestData()
                {
                    Quantity = 2,
                    Unit = MeasurementUnit.Kilogram
                };

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.EditAsync(
                    1,
                    request,
                    "user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Status, Is.True);
                Assert.That(pantryItem.Quantity, Is.EqualTo(2));
                Assert.That(pantryItem.Unit, Is.EqualTo(MeasurementUnit.Kilogram));
            });
        }

        [Test]
        [Property("Requirement", "REQ-PS-EDIT-02")]
        public async Task EditAsync_InvalidRequest_ReturnsValidationErrors_Negative()
        {
            EditPantryItemRequestData request =
                new EditPantryItemRequestData()
                {
                    Quantity = 0,
                    Unit = MeasurementUnit.Gram
                };

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.EditAsync(
                    1,
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(actualResult.ErrorMessages, Is.Not.Empty);
        }

        [Test]
        [Property("Requirement", "REQ-PS-EDIT-03")]
        public async Task EditAsync_ItemDoesNotBelongToUser_ReturnsNotFound_Negative()
        {
            PantryItem pantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-2",
                    IngredientId = 1,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        pantryItem
                    }.BuildMock());

            EditPantryItemRequestData request =
                new EditPantryItemRequestData()
                {
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                };

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.EditAsync(
                    1,
                    request,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain("Pantry item not found."));
        }

        [Test]
        [Property("Requirement", "REQ-PS-DELETE-01")]
        public async Task DeleteAsync_ValidOwnedItem_RemovesItemAndReturnsSuccess_Positive()
        {
            PantryItem pantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        pantryItem
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.Remove(pantryItem));

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.DeleteAsync(
                    1,
                    "user-1");

            Assert.That(actualResult.Status, Is.True);

            this.pantryItemRepositoryMock
                .Verify(r => r.Remove(pantryItem), Times.Once);
        }

        [Test]
        [Property("Requirement", "REQ-PS-DELETE-02")]
        public async Task DeleteAsync_ItemDoesNotBelongToUser_ReturnsNotFound_Negative()
        {
            PantryItem pantryItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-2",
                    IngredientId = 1,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        pantryItem
                    }.BuildMock());

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.DeleteAsync(
                    1,
                    "user-1");

            Assert.That(actualResult.Status, Is.False);
            Assert.That(
                actualResult.ErrorMessages,
                Does.Contain("Pantry item not found."));
        }

        [Test]
        [Property("Requirement", "REQ-PS-CLEAR-01")]
        public async Task ClearAsync_RemovesOnlyCurrentUsersItems_Positive()
        {
            PantryItem firstUserItem =
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                };

            PantryItem secondUserItem =
                new PantryItem()
                {
                    Id = 2,
                    UserId = "user-1",
                    IngredientId = 2,
                    Quantity = 200,
                    Unit = MeasurementUnit.Gram
                };

            PantryItem otherUserItem =
                new PantryItem()
                {
                    Id = 3,
                    UserId = "user-2",
                    IngredientId = 3,
                    Quantity = 300,
                    Unit = MeasurementUnit.Gram
                };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(
                    new List<PantryItem>()
                    {
                        firstUserItem,
                        secondUserItem,
                        otherUserItem
                    }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.Remove(firstUserItem));

            this.pantryItemRepositoryMock
                .Setup(r => r.Remove(secondUserItem));

            this.pantryItemRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(2);

            PantryServiceStatusResponseModel actualResult =
                await this.pantryService.ClearAsync("user-1");

            Assert.That(actualResult.Status, Is.True);

            this.pantryItemRepositoryMock
                .Verify(r => r.Remove(firstUserItem), Times.Once);

            this.pantryItemRepositoryMock
                .Verify(r => r.Remove(secondUserItem), Times.Once);

            this.pantryItemRepositoryMock
                .Verify(r => r.Remove(otherUserItem), Times.Never);
        }
    }
}

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
    using Services.Core.Data.Pantry.Response;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class PantryServiceTests
    {
        private Mock<IPantryItemRepository> pantryItemRepositoryMock = null!;
        private Mock<IIngredientRepository> ingredientRepositoryMock = null!;
        private Mock<ILogger<PantryService>> loggerMock = null!;
        private BasicModelValidator modelValidator = null!;

        [SetUp]
        public void SetUp()
        {
            this.pantryItemRepositoryMock = new Mock<IPantryItemRepository>();
            this.ingredientRepositoryMock = new Mock<IIngredientRepository>();
            this.loggerMock = new Mock<ILogger<PantryService>>();
            this.modelValidator = new BasicModelValidator();
        }

        [Test]
        [Property("Requirement", "REQ-PS-GETALL-01")]
        public async Task GetAllAsync_ReturnsOnlyCurrentUsersItemsOrderedByIngredientName_Positive()
        {
            Ingredient rice = new Ingredient() { Id = 1, Name = "Rice" };
            Ingredient apple = new Ingredient() { Id = 2, Name = "Apple" };
            Ingredient tomato = new Ingredient() { Id = 3, Name = "Tomato" };

            List<PantryItem> mockPantryItems = new List<PantryItem>()
            {
                new PantryItem()
                {
                    Id = 1,
                    UserId = "user-1",
                    IngredientId = 1,
                    Ingredient = rice,
                    Quantity = 500,
                    Unit = MeasurementUnit.Gram
                },
                new PantryItem()
                {
                    Id = 2,
                    UserId = "user-1",
                    IngredientId = 2,
                    Ingredient = apple,
                    Quantity = 2,
                    Unit = MeasurementUnit.Piece
                },
                new PantryItem()
                {
                    Id = 3,
                    UserId = "user-2",
                    IngredientId = 3,
                    Ingredient = tomato,
                    Quantity = 100,
                    Unit = MeasurementUnit.Gram
                }
            };

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockPantryItems.BuildMock());

            IPantryService pantryService = this.CreatePantryService();

            List<PantryItemResponseModel> actualResult =
                await pantryService.GetAllAsync("user-1");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Count, Is.EqualTo(2));
                Assert.That(actualResult[0].IngredientName, Is.EqualTo("Apple"));
                Assert.That(actualResult[1].IngredientName, Is.EqualTo("Rice"));
                Assert.That(actualResult.All(p => p.Id != 3), Is.True);
            });
        }

        [Test]
        [Property("Requirement", "REQ-PS-SEARCH-01")]
        public async Task SearchIngredientsAsync_EmptySearch_ReturnsEmptyCollection_Positive()
        {
            IPantryService pantryService = this.CreatePantryService();

            List<DropdownResponseModel> actualResult =
                await pantryService.SearchIngredientsAsync("   ");

            Assert.NotNull(actualResult);
            Assert.Zero(actualResult.Count);
        }

        [Test]
        [Property("Requirement", "REQ-PS-SEARCH-02")]
        public async Task SearchIngredientsAsync_ReturnsOnlyUsedMatchingIngredientsOrdered_Positive()
        {
            Dish dish = new Dish()
            {
                Id = 1,
                Name = "Dish",
                Ingredients = "Ingredients",
                Instructions = "Instructions",
                Calories = 300,
                PreparationTime = 20,
                Portions = 1,
                DishType = DishType.Meat
            };

            Ingredient rice = new Ingredient() { Id = 1, Name = "Rice" };
            Ingredient brownRice = new Ingredient() { Id = 2, Name = "Brown Rice" };
            Ingredient unusedRice = new Ingredient() { Id = 3, Name = "Rice Flour" };
            Ingredient tomato = new Ingredient() { Id = 4, Name = "Tomato" };

            rice.DishIngredients.Add(new DishIngredient()
            {
                Id = 1,
                Dish = dish,
                Ingredient = rice,
                Quantity = 100,
                Unit = MeasurementUnit.Gram
            });

            brownRice.DishIngredients.Add(new DishIngredient()
            {
                Id = 2,
                Dish = dish,
                Ingredient = brownRice,
                Quantity = 100,
                Unit = MeasurementUnit.Gram
            });

            List<Ingredient> mockIngredients = new List<Ingredient>()
            {
                rice,
                brownRice,
                unusedRice,
                tomato
            };

            this.ingredientRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(mockIngredients.BuildMock());

            IPantryService pantryService = this.CreatePantryService();

            List<DropdownResponseModel> actualResult =
                await pantryService.SearchIngredientsAsync("Rice");

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.Count, Is.EqualTo(2));
                Assert.That(actualResult[0].Name, Is.EqualTo("Brown Rice"));
                Assert.That(actualResult[1].Name, Is.EqualTo("Rice"));
            });
        }

        [Test]
        [Property("Requirement", "REQ-PS-UNITS-01")]
        public void LoadMeasurementUnits_ReturnsAllMeasurementUnits_Positive()
        {
            IPantryService pantryService = this.CreatePantryService();

            List<DropdownResponseModel> actualResult =
                pantryService.LoadMeasurementUnits().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(
                    actualResult.Count,
                    Is.EqualTo(Enum.GetValues<MeasurementUnit>().Length));

                Assert.That(
                    actualResult.Any(x =>
                        x.Id == (int)MeasurementUnit.Gram &&
                        x.Name == MeasurementUnit.Gram.ToString()),
                    Is.True);

                Assert.That(
                    actualResult.Any(x =>
                        x.Id == (int)MeasurementUnit.Liter &&
                        x.Name == MeasurementUnit.Liter.ToString()),
                    Is.True);

                Assert.That(
                    actualResult.Any(x =>
                        x.Id == (int)MeasurementUnit.Piece &&
                        x.Name == MeasurementUnit.Piece.ToString()),
                    Is.True);
            });
        }

        private IPantryService CreatePantryService()
        {
            return new PantryService(
                this.pantryItemRepositoryMock.Object,
                this.ingredientRepositoryMock.Object,
                this.modelValidator,
                this.loggerMock.Object);
        }
    }
}

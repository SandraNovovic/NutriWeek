namespace NutriWeek.Services.Tests.DishServiceTest
{
    using Microsoft.Extensions.Logging;
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using GCommon.Utilities;
    using Services.Core.Data.Common.Response;
    using Services.Core.Data.Dish.Request;
    using Services.Core.Data.Dish.Response;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class DishServiceTests
    {
        private Mock<ILogger<DishService>> loggerMock = null!;
        private Mock<IDishRepository> dishRepositoryMock = null!;
        private Mock<IIngredientRepository> ingredientRepositoryMock = null!;
        private Mock<IDishIngredientRepository> dishIngredientRepositoryMock = null!;
        private BasicModelValidator modelValidator = null!;
        private IDishService dishService = null!;

        [SetUp]
        public void SetUp()
        {
            loggerMock = new Mock<ILogger<DishService>>();
            dishRepositoryMock = new Mock<IDishRepository>();
            ingredientRepositoryMock = new Mock<IIngredientRepository>();
            dishIngredientRepositoryMock = new Mock<IDishIngredientRepository>();
            modelValidator = new BasicModelValidator();
            dishService = new DishService(
                dishRepositoryMock.Object,
                modelValidator,
                loggerMock.Object,
                ingredientRepositoryMock.Object,
                dishIngredientRepositoryMock.Object);
        }

        [Test]
        [Property("Requirement", "REQ-DS-GETALL-01")]
        [Property("Requirement", "REQ-DS-GETALL-02")]
        [Property("Requirement", "REQ-DS-GETALL-03")]
        public async Task GetAllAsync_ReturnsOnlyOwnAndSharedNonDeletedDishes_Positive()
        {
            string userId = "user-1";

            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Chicken Rice Bowl", userId, false, false, DishType.Meat, 520, 30),
                CreateDish(2, "Shared Vegan Pasta", null, true, false, DishType.Vegan, 430, 25),
                CreateDish(3, "Other User Dish", "user-2", false, false, DishType.Vegetarian, 400, 20),
                CreateDish(4, "Deleted Dish", userId, false, true, DishType.Fish, 350, 15)
            };
            IQueryable<Dish> dishesQueryableMock = mockDishes.BuildMock();

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(dishesQueryableMock);

            IEnumerable<GetAllDishesResponseModel> expectedResult = mockDishes
                .Where(d => !d.IsDeleted && (d.IsShared || d.UserId == userId))
                .Select(d => new GetAllDishesResponseModel()
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Calories = d.Calories,
                    PreparationTime = d.PreparationTime,
                    Portions = d.Portions,
                    ImageUrl = d.ImageUrl,
                    DishType = d.DishType,
                    IsShared = d.IsShared
                })
                .ToArray();

            IEnumerable<GetAllDishesResponseModel> actualResult =
                await dishService.GetAllAsync(userId);

            Assert.That(actualResult.Count(), Is.EqualTo(expectedResult.Count()));

            for (int i = 0; i < expectedResult.Count(); i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(actualResult.ElementAt(i).Id, Is.EqualTo(expectedResult.ElementAt(i).Id));
                    Assert.That(actualResult.ElementAt(i).Name, Is.EqualTo(expectedResult.ElementAt(i).Name));
                    Assert.That(actualResult.ElementAt(i).Description, Is.EqualTo(expectedResult.ElementAt(i).Description));
                    Assert.That(actualResult.ElementAt(i).Calories, Is.EqualTo(expectedResult.ElementAt(i).Calories));
                    Assert.That(actualResult.ElementAt(i).PreparationTime, Is.EqualTo(expectedResult.ElementAt(i).PreparationTime));
                    Assert.That(actualResult.ElementAt(i).Portions, Is.EqualTo(expectedResult.ElementAt(i).Portions));
                    Assert.That(actualResult.ElementAt(i).DishType, Is.EqualTo(expectedResult.ElementAt(i).DishType));
                    Assert.That(actualResult.ElementAt(i).IsShared, Is.EqualTo(expectedResult.ElementAt(i).IsShared));
                });
            }
        }

        [Test]
        [Property("Requirement", "REQ-DS-GETALL-04")]
        public async Task GetAllAsync_ReturnsEmptyCollectionWhenNoDishesInRepository_Positive()
        {
            List<Dish> mockDishes = new List<Dish>();
            IQueryable<Dish> emptyDishesQueryableMock = mockDishes.BuildMock();

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(emptyDishesQueryableMock);

            IEnumerable<GetAllDishesResponseModel> actualResult =
                await dishService.GetAllAsync("user-1");

            Assert.NotNull(actualResult);
            Assert.Zero(actualResult.Count());
        }


        [Test]
        [Property("Requirement", "REQ-DS-GETPAGED-01")]
        public async Task GetPagedAsync_SearchAndDishType_ReturnsFilteredPagedResult_Positive()
        {
            string userId = "user-1";

            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Chicken Bowl", userId, false, false, DishType.Meat, 500, 30, "Chicken dinner"),
                CreateDish(2, "Chicken Salad", null, true, false, DishType.Meat, 350, 20, "Fresh chicken salad"),
                CreateDish(3, "Vegan Bowl", userId, false, false, DishType.Vegan, 300, 10, "Healthy vegan meal"),
                CreateDish(4, "Chicken Other", "user-2", false, false, DishType.Meat, 250, 20, "Other user"),
                CreateDish(5, "Deleted Fish", userId, false, true, DishType.Fish, 600, 40, "Deleted"),
                CreateDish(6, "Fish Grill", null, true, false, DishType.Fish, 700, 45, "Shared fish")
            };

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            GetDishesPageRequestData request = new GetDishesPageRequestData()
            {
                Search = "Chicken",
                DishType = DishType.Meat,
                SortBy = "nameAsc",
                Page = 1,
                PageSize = 1
            };

            GetDishesPageResponseModel actualResult =
                await dishService.GetPagedAsync(request, userId);

            Assert.Multiple(() =>
            {
                Assert.That(actualResult.TotalItems, Is.EqualTo(2));
                Assert.That(actualResult.TotalPages, Is.EqualTo(2));
                Assert.That(actualResult.CurrentPage, Is.EqualTo(1));
                Assert.That(actualResult.Dishes.Count, Is.EqualTo(1));
                Assert.That(actualResult.Dishes[0].Name, Is.EqualTo("Chicken Bowl"));
                Assert.That(actualResult.MeatCount, Is.EqualTo(2));
                Assert.That(actualResult.VeganCount, Is.EqualTo(1));
                Assert.That(actualResult.FishCount, Is.EqualTo(1));
                Assert.That(actualResult.VegetarianCount, Is.EqualTo(0));
            });
        }

        [TestCase("under15", 1)]
        [Property("Requirement", "REQ-DS-GETPAGED-02")]
        public async Task GetPagedAsync_CookingTimeFilter_ReturnsExpectedDish_Positive( string cookingTime,int expectedId)
        {
            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Quick Dish", "user-1", false, false, DishType.Meat, 300, 10),
                CreateDish(2, "Medium Dish", "user-1", false, false, DishType.Meat, 400, 20),
                CreateDish(3, "Long Dish", "user-1", false, false, DishType.Meat, 500, 40)
            };

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            GetDishesPageRequestData request = new GetDishesPageRequestData()
            {
                CookingTime = cookingTime,
                Page = 1,
                PageSize = 8
            };

            GetDishesPageResponseModel actualResult =
                await dishService.GetPagedAsync(request, "user-1");

            Assert.That(actualResult.Dishes.Count, Is.EqualTo(1));
            Assert.That(actualResult.Dishes[0].Id, Is.EqualTo(expectedId));
        }

        [TestCase("under350", 1)]
        [Property("Requirement", "REQ-DS-GETPAGED-03")]
        public async Task GetPagedAsync_CaloriesFilter_ReturnsExpectedDish_Positive(  string calories, int expectedId)
        {
            List<Dish> mockDishes = new List<Dish>()
            {
                CreateDish(1, "Dish 1", "user-1", false, false, DishType.Meat, 300, 20),
                CreateDish(2, "Dish 2", "user-1", false, false, DishType.Meat, 400, 20),
                CreateDish(3, "Dish 3", "user-1", false, false, DishType.Meat, 500, 20),
                CreateDish(4, "Dish 4", "user-1", false, false, DishType.Meat, 600, 20)
            };

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(mockDishes.BuildMock());

            GetDishesPageRequestData request = new GetDishesPageRequestData()
            {
                Calories = calories,
                Page = 1,
                PageSize = 8
            };

            GetDishesPageResponseModel actualResult =
                await dishService.GetPagedAsync(request, "user-1");

            Assert.That(actualResult.Dishes.Count, Is.EqualTo(1));
            Assert.That(actualResult.Dishes[0].Id, Is.EqualTo(expectedId));
        }





        [Test]
        [Property("Requirement", "REQ-DS-TYPES-01")]
        public void LoadDishTypes_ReturnsAllDishTypes_Positive()
        {
            List<DropdownResponseModel> actualResult =
                dishService.LoadDishTypes().ToList();

            Assert.That(actualResult.Count, Is.EqualTo(Enum.GetValues<DishType>().Length));
            Assert.That(actualResult.Select(x => x.Id), Is.EquivalentTo(Enum.GetValues<DishType>().Select(x => (int)x)));
            Assert.That(actualResult.Select(x => x.Name), Is.EquivalentTo(Enum.GetNames<DishType>()));
        }

        [TestCase(DishType.Meat)]
        [Property("Requirement", "REQ-DS-DISHTYPE-01")]
        public void IsDishTypeValid_DefinedDishType_ReturnsTrue_Positive(DishType dishType)
        {
            bool actualResult = dishService.IsDishTypeValid(dishType);

            Assert.IsTrue(actualResult);
        }

        [Test]
        [Property("Requirement", "REQ-DS-DISHTYPE-02")]
        public void IsDishTypeValid_UndefinedDishType_ReturnsFalse_Negative()
        {
            bool actualResult = dishService.IsDishTypeValid((DishType)999);

            Assert.IsFalse(actualResult);
        }

        [Test]
        [Property("Requirement", "REQ-DS-EDIT-LOAD-01")]
        public async Task GetDishForEditAsync_OwnedDish_ReturnsMappedDishAndIngredients_Positive()
        {
            Ingredient rice = new Ingredient()
            {
                Id = 1,
                Name = "Rice"
            };

            Dish dish = CreateDish(1, "Chicken Rice", "user-1", false, false, DishType.Meat, 500, 30);
            dish.DishIngredients.Add(new DishIngredient()
            {
                Id = 1,
                Dish = dish,
                DishId = dish.Id,
                Ingredient = rice,
                IngredientId = rice.Id,
                Quantity = 100,
                Unit = MeasurementUnit.Gram
            });

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());

            GetDishForEditResponseModel? actualResult =
                await dishService.GetDishForEditAsync(1, "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(actualResult!.Id, Is.EqualTo(dish.Id));
                Assert.That(actualResult.Name, Is.EqualTo(dish.Name));
                Assert.That(actualResult.DishType, Is.EqualTo(dish.DishType));
                Assert.That(actualResult.IngredientItems.Count, Is.EqualTo(1));
                Assert.That(actualResult.IngredientItems[0].Name, Is.EqualTo("Rice"));
                Assert.That(actualResult.IngredientItems[0].Quantity, Is.EqualTo(100));
                Assert.That(actualResult.IngredientItems[0].Unit, Is.EqualTo(MeasurementUnit.Gram));
            });
        }




        [Test]
        [Property("Requirement", "REQ-DS-DETAILS-02")]
        public async Task GetDishDetailsAsync_SharedDish_ReturnsDishForOtherUser_Positive()
        {
            Dish dish = CreateDish(1, "Shared Dish", null, true, false, DishType.Vegan, 300, 20);

            dishRepositoryMock.Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<Dish>() { dish }.BuildMock());

            GetDishDetailsResponseModel? actualResult =
                await dishService.GetDishDetailsAsync(1, "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.IsTrue(actualResult!.IsShared);
        }



        private static Dish CreateDish(
            int id,
            string name,
            string? userId,
            bool isShared,
            bool isDeleted,
            DishType dishType,
            int calories,
            int preparationTime,
            string description = "Valid dish description")
        {
            return new Dish()
            {
                Id = id,
                Name = name,
                Description = description,
                Ingredients = "Valid ingredients",
                Instructions = "Valid cooking instructions",
                Calories = calories,
                PreparationTime = preparationTime,
                Portions = 2,
                DishType = dishType,
                UserId = userId,
                IsShared = isShared,
                IsDeleted = isDeleted
            };
        }
    }
}

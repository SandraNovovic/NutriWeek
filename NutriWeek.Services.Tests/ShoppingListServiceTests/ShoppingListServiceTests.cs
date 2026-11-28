namespace NutriWeek.Services.Tests
{
    using MockQueryable;
    using Moq;

    using Data.Models;
    using Data.Models.Enums;
    using Data.Repository.Interfaces;
    using Services.Core.Data.ShoppingList.Response;
    using Services.Core.Interfaces;
    using Services.Core.Services;

    [TestFixture]
    public class ShoppingListServiceTests
    {
        private Mock<IWeeklyMenuRepository> weeklyMenuRepositoryMock = null!;
        private Mock<IDailyMenuRepository> dailyMenuRepositoryMock = null!;
        private Mock<IPantryItemRepository> pantryItemRepositoryMock = null!;
        private IShoppingListService shoppingListService = null!;

        [SetUp]
        public void SetUp()
        {
            this.weeklyMenuRepositoryMock = new Mock<IWeeklyMenuRepository>();
            this.dailyMenuRepositoryMock = new Mock<IDailyMenuRepository>();
            this.pantryItemRepositoryMock = new Mock<IPantryItemRepository>();
            this.shoppingListService = new ShoppingListService(
                this.weeklyMenuRepositoryMock.Object,
                this.dailyMenuRepositoryMock.Object,
                this.pantryItemRepositoryMock.Object);
        }

        [Test]
        [Property("Requirement", "REQ-SLS-WEEK-01")]
        public async Task GetForWeekAsync_ReturnsNullWhenWeeklyMenuDoesNotExist_Negative()
        {
            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<WeeklyMenu>().BuildMock());

            ShoppingListResponseModel? actualResult =
                await this.shoppingListService.GetForWeekAsync(1, "user-1");

            Assert.That(actualResult, Is.Null);
        }

        [TestCase("user-2", false)]
        [TestCase("user-1", true)]
        [Property("Requirement", "REQ-SLS-WEEK-02")]
        public async Task GetForWeekAsync_ReturnsNullForUnauthorizedOrDeletedMenu_Negative( string menuUserId,bool isDeleted)
        {
            WeeklyMenu weeklyMenu = new WeeklyMenu()
            {
                Id = 1,
                Title = "Week",
                WeekStartDate = new DateTime(2026, 11, 23),
                UserId = menuUserId,
                IsDeleted = isDeleted
            };

            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());

            ShoppingListResponseModel? actualResult =
                await this.shoppingListService.GetForWeekAsync(1, "user-1");

            Assert.That(actualResult, Is.Null);
        }

        [Test]
        [Property("Requirement", "REQ-SLS-WEEK-03")]
        [Property("Requirement", "REQ-SLS-WEEK-04")]
        [Property("Requirement", "REQ-SLS-WEEK-05")]
        public async Task GetForWeekAsync_AggregatesIngredientsConvertsUnitsAndSubtractsPantry_Positive()
        {
            Ingredient flour = CreateIngredient(1, "Flour");
            Ingredient milk = CreateIngredient(2, "Milk");
            Ingredient eggs = CreateIngredient(3, "Eggs");

            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            DailyMenu dailyMenu = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24));

            Dish firstDish = CreateDish(1, "Breakfast");
            AddIngredient(firstDish, flour, 0.5m, MeasurementUnit.Kilogram);
            AddIngredient(firstDish, milk, 1m, MeasurementUnit.Liter);
            AddIngredient(firstDish, eggs, 2m, MeasurementUnit.Piece);

            Dish secondDish = CreateDish(2, "Snack");
            AddIngredient(secondDish, flour, 250m, MeasurementUnit.Gram);
            AddIngredient(secondDish, milk, 250m, MeasurementUnit.Milliliter);

            dailyMenu.Meals.Add(CreateMeal(1, dailyMenu, firstDish, false));
            dailyMenu.Meals.Add(CreateMeal(2, dailyMenu, secondDish, false));
            weeklyMenu.DailyMenus.Add(dailyMenu);

            List<PantryItem> pantryItems = new List<PantryItem>()
            {
                CreatePantryItem(1, "user-1", flour, 200m, MeasurementUnit.Gram),
                CreatePantryItem(2, "user-1", milk, 0.5m, MeasurementUnit.Liter),
                CreatePantryItem(3, "user-1", eggs, 1m, MeasurementUnit.Piece)
            };

            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(pantryItems.BuildMock());

            ShoppingListResponseModel? actualResult =
                await this.shoppingListService.GetForWeekAsync(1, "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.WeeklyMenuId, Is.EqualTo(1));
            Assert.That(actualResult.WeekStartDate, Is.EqualTo(new DateTime(2026, 11, 23)));
            Assert.That(actualResult.Items.Count, Is.EqualTo(3));
            Assert.That(actualResult.Items.Select(i => i.IngredientName),
                Is.EqualTo(new[] { "Eggs", "Flour", "Milk" }));

            ShoppingListItemResponseModel flourItem = actualResult.Items.Single(i => i.IngredientId == 1);
            ShoppingListItemResponseModel milkItem = actualResult.Items.Single(i => i.IngredientId == 2);
            ShoppingListItemResponseModel eggsItem = actualResult.Items.Single(i => i.IngredientId == 3);

            Assert.Multiple(() =>
            {
                Assert.That(flourItem.RequiredQuantity, Is.EqualTo(750m));
                Assert.That(flourItem.PantryQuantity, Is.EqualTo(200m));
                Assert.That(flourItem.QuantityToBuy, Is.EqualTo(550m));
                Assert.That(flourItem.Unit, Is.EqualTo(MeasurementUnit.Gram));

                Assert.That(milkItem.RequiredQuantity, Is.EqualTo(1250m));
                Assert.That(milkItem.PantryQuantity, Is.EqualTo(500m));
                Assert.That(milkItem.QuantityToBuy, Is.EqualTo(750m));
                Assert.That(milkItem.Unit, Is.EqualTo(MeasurementUnit.Milliliter));

                Assert.That(eggsItem.RequiredQuantity, Is.EqualTo(2m));
                Assert.That(eggsItem.PantryQuantity, Is.EqualTo(1m));
                Assert.That(eggsItem.QuantityToBuy, Is.EqualTo(1m));
                Assert.That(eggsItem.Unit, Is.EqualTo(MeasurementUnit.Piece));
            });
        }

        [Test]
        [Property("Requirement", "REQ-SLS-WEEK-06")]
        public async Task GetForWeekAsync_IgnoresDeletedDailyMenusAndMeals_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");
            Ingredient chicken = CreateIngredient(2, "Chicken");
            Ingredient milk = CreateIngredient(3, "Milk");

            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));

            DailyMenu activeDay = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24));
            Dish riceDish = CreateDish(1, "Rice Dish");
            AddIngredient(riceDish, rice, 100m, MeasurementUnit.Gram);
            activeDay.Meals.Add(CreateMeal(1, activeDay, riceDish, false));

            Dish deletedMealDish = CreateDish(2, "Chicken Dish");
            AddIngredient(deletedMealDish, chicken, 200m, MeasurementUnit.Gram);
            activeDay.Meals.Add(CreateMeal(2, activeDay, deletedMealDish, true));

            DailyMenu deletedDay = CreateDailyMenu(2, weeklyMenu, new DateTime(2026, 11, 25), true);
            Dish milkDish = CreateDish(3, "Milk Dish");
            AddIngredient(milkDish, milk, 1m, MeasurementUnit.Liter);
            deletedDay.Meals.Add(CreateMeal(3, deletedDay, milkDish, false));

            weeklyMenu.DailyMenus.Add(activeDay);
            weeklyMenu.DailyMenus.Add(deletedDay);

            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>().BuildMock());

            ShoppingListResponseModel? actualResult =
                await this.shoppingListService.GetForWeekAsync(1, "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Items.Count, Is.EqualTo(1));
            Assert.That(actualResult.Items[0].IngredientName, Is.EqualTo("Rice"));
            Assert.That(actualResult.Items[0].RequiredQuantity, Is.EqualTo(100m));
        }

        [Test]
        [Property("Requirement", "REQ-SLS-WEEK-07")]
        public async Task GetForWeekAsync_QuantityToBuyDoesNotBecomeNegative_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");
            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            DailyMenu dailyMenu = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24));
            Dish dish = CreateDish(1, "Rice Dish");
            AddIngredient(dish, rice, 100m, MeasurementUnit.Gram);
            dailyMenu.Meals.Add(CreateMeal(1, dailyMenu, dish, false));
            weeklyMenu.DailyMenus.Add(dailyMenu);

            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<WeeklyMenu>() { weeklyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>()
                {
                    CreatePantryItem(1, "user-1", rice, 500m, MeasurementUnit.Gram)
                }.BuildMock());

            ShoppingListResponseModel? actualResult =
                await this.shoppingListService.GetForWeekAsync(1, "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Items.Single().QuantityToBuy, Is.Zero);
        }

        [Test]
        [Property("Requirement", "REQ-SLS-WEEK-08")]
        public void GetForWeekAsync_RepositoryThrows_ThrowsException_Negative()
        {
            this.weeklyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Throws(new InvalidOperationException("boom"));

            Assert.ThrowsAsync<InvalidOperationException>(
                () => this.shoppingListService.GetForWeekAsync(1, "user-1"));
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-01")]
        public async Task GetForDayAsync_ReturnsNullWhenDailyMenuDoesNotExist_Negative()
        {
            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<DailyMenu>().BuildMock());

            List<ShoppingListItemResponseModel>? actualResult =
                await this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1");

            Assert.That(actualResult, Is.Null);
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-02")]
        [Property("Requirement", "REQ-SLS-DAY-03")]
        public async Task GetForDayAsync_AggregatesIngredientsConvertsUnitsAndSubtractsPantry_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");
            Ingredient milk = CreateIngredient(2, "Milk");

            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            DailyMenu dailyMenu = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24, 15, 30, 0));

            Dish firstDish = CreateDish(1, "First Dish");
            AddIngredient(firstDish, rice, 0.5m, MeasurementUnit.Kilogram);
            AddIngredient(firstDish, milk, 500m, MeasurementUnit.Milliliter);

            Dish secondDish = CreateDish(2, "Second Dish");
            AddIngredient(secondDish, rice, 250m, MeasurementUnit.Gram);
            AddIngredient(secondDish, milk, 1m, MeasurementUnit.Liter);

            dailyMenu.Meals.Add(CreateMeal(1, dailyMenu, firstDish, false));
            dailyMenu.Meals.Add(CreateMeal(2, dailyMenu, secondDish, false));

            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<DailyMenu>() { dailyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>()
                {
                    CreatePantryItem(1, "user-1", rice, 300m, MeasurementUnit.Gram),
                    CreatePantryItem(2, "user-1", milk, 0.5m, MeasurementUnit.Liter)
                }.BuildMock());

            List<ShoppingListItemResponseModel>? actualResult =
                await this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Count, Is.EqualTo(2));
            Assert.That(actualResult.Select(i => i.IngredientName),
                Is.EqualTo(new[] { "Milk", "Rice" }));

            ShoppingListItemResponseModel riceItem = actualResult.Single(i => i.IngredientId == 1);
            ShoppingListItemResponseModel milkItem = actualResult.Single(i => i.IngredientId == 2);

            Assert.Multiple(() =>
            {
                Assert.That(riceItem.RequiredQuantity, Is.EqualTo(750m));
                Assert.That(riceItem.PantryQuantity, Is.EqualTo(300m));
                Assert.That(riceItem.QuantityToBuy, Is.EqualTo(450m));
                Assert.That(riceItem.Unit, Is.EqualTo(MeasurementUnit.Gram));

                Assert.That(milkItem.RequiredQuantity, Is.EqualTo(1500m));
                Assert.That(milkItem.PantryQuantity, Is.EqualTo(500m));
                Assert.That(milkItem.QuantityToBuy, Is.EqualTo(1000m));
                Assert.That(milkItem.Unit, Is.EqualTo(MeasurementUnit.Milliliter));
            });
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-04")]
        public async Task GetForDayAsync_IgnoresDeletedMeals_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");
            Ingredient chicken = CreateIngredient(2, "Chicken");

            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            DailyMenu dailyMenu = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24));

            Dish activeDish = CreateDish(1, "Rice Dish");
            AddIngredient(activeDish, rice, 100m, MeasurementUnit.Gram);
            dailyMenu.Meals.Add(CreateMeal(1, dailyMenu, activeDish, false));

            Dish deletedDish = CreateDish(2, "Chicken Dish");
            AddIngredient(deletedDish, chicken, 200m, MeasurementUnit.Gram);
            dailyMenu.Meals.Add(CreateMeal(2, dailyMenu, deletedDish, true));

            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<DailyMenu>() { dailyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>().BuildMock());

            List<ShoppingListItemResponseModel>? actualResult =
                await this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Count, Is.EqualTo(1));
            Assert.That(actualResult[0].IngredientName, Is.EqualTo("Rice"));
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-05")]
        public async Task GetForDayAsync_FiltersByDateAndUser_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");

            WeeklyMenu ownWeeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            WeeklyMenu otherWeeklyMenu = CreateWeeklyMenu(2, "user-2", new DateTime(2026, 11, 23));

            DailyMenu ownTargetDay = CreateDailyMenu(1, ownWeeklyMenu, new DateTime(2026, 11, 24, 18, 0, 0));
            Dish ownDish = CreateDish(1, "Own Dish");
            AddIngredient(ownDish, rice, 100m, MeasurementUnit.Gram);
            ownTargetDay.Meals.Add(CreateMeal(1, ownTargetDay, ownDish, false));

            DailyMenu ownOtherDay = CreateDailyMenu(2, ownWeeklyMenu, new DateTime(2026, 11, 25));
            DailyMenu otherUserDay = CreateDailyMenu(3, otherWeeklyMenu, new DateTime(2026, 11, 24));

            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<DailyMenu>()
                {
                    ownOtherDay,
                    otherUserDay,
                    ownTargetDay
                }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>().BuildMock());

            List<ShoppingListItemResponseModel>? actualResult =
                await this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Count, Is.EqualTo(1));
            Assert.That(actualResult[0].IngredientName, Is.EqualTo("Rice"));
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-06")]
        public async Task GetForDayAsync_QuantityToBuyDoesNotBecomeNegative_Positive()
        {
            Ingredient rice = CreateIngredient(1, "Rice");
            WeeklyMenu weeklyMenu = CreateWeeklyMenu(1, "user-1", new DateTime(2026, 11, 23));
            DailyMenu dailyMenu = CreateDailyMenu(1, weeklyMenu, new DateTime(2026, 11, 24));
            Dish dish = CreateDish(1, "Rice Dish");
            AddIngredient(dish, rice, 100m, MeasurementUnit.Gram);
            dailyMenu.Meals.Add(CreateMeal(1, dailyMenu, dish, false));

            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<DailyMenu>() { dailyMenu }.BuildMock());

            this.pantryItemRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Returns(new List<PantryItem>()
                {
                    CreatePantryItem(1, "user-1", rice, 1m, MeasurementUnit.Kilogram)
                }.BuildMock());

            List<ShoppingListItemResponseModel>? actualResult =
                await this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1");

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult!.Single().PantryQuantity, Is.EqualTo(1000m));
            Assert.That(actualResult.Single().QuantityToBuy, Is.Zero);
        }

        [Test]
        [Property("Requirement", "REQ-SLS-DAY-07")]
        public void GetForDayAsync_RepositoryThrows_ThrowsException_Negative()
        {
            this.dailyMenuRepositoryMock
                .Setup(r => r.GetAllAsNoTracking())
                .Throws(new InvalidOperationException("boom"));

            Assert.ThrowsAsync<InvalidOperationException>(
                () => this.shoppingListService.GetForDayAsync(
                    new DateTime(2026, 11, 24),
                    "user-1"));
        }

        private static WeeklyMenu CreateWeeklyMenu(
            int id,
            string userId,
            DateTime weekStartDate)
        {
            return new WeeklyMenu()
            {
                Id = id,
                Title = "Week",
                WeekStartDate = weekStartDate,
                UserId = userId,
                IsDeleted = false
            };
        }

        private static DailyMenu CreateDailyMenu(
            int id,
            WeeklyMenu weeklyMenu,
            DateTime date,
            bool isDeleted = false)
        {
            return new DailyMenu()
            {
                Id = id,
                Date = date,
                WeeklyMenuId = weeklyMenu.Id,
                WeeklyMenu = weeklyMenu,
                IsDeleted = isDeleted
            };
        }

        private static Meal CreateMeal(
            int id,
            DailyMenu dailyMenu,
            Dish dish,
            bool isDeleted)
        {
            return new Meal()
            {
                Id = id,
                DailyMenuId = dailyMenu.Id,
                DailyMenu = dailyMenu,
                DishId = dish.Id,
                Dish = dish,
                IsDeleted = isDeleted
            };
        }

        private static Dish CreateDish(
            int id,
            string name)
        {
            return new Dish()
            {
                Id = id,
                Name = name,
                Description = "Description",
                Ingredients = "Ingredients",
                Instructions = "Instructions",
                Calories = 300,
                PreparationTime = 20,
                Portions = 2,
                DishType = DishType.Meat,
                IsDeleted = false
            };
        }

        private static Ingredient CreateIngredient(
            int id,
            string name)
        {
            return new Ingredient()
            {
                Id = id,
                Name = name
            };
        }

        private static void AddIngredient(
            Dish dish,
            Ingredient ingredient,
            decimal quantity,
            MeasurementUnit unit)
        {
            DishIngredient dishIngredient = new DishIngredient()
            {
                Id = dish.DishIngredients.Count + 1,
                DishId = dish.Id,
                Dish = dish,
                IngredientId = ingredient.Id,
                Ingredient = ingredient,
                Quantity = quantity,
                Unit = unit
            };

            dish.DishIngredients.Add(dishIngredient);
        }

        private static PantryItem CreatePantryItem(
            int id,
            string userId,
            Ingredient ingredient,
            decimal quantity,
            MeasurementUnit unit)
        {
            return new PantryItem()
            {
                Id = id,
                UserId = userId,
                IngredientId = ingredient.Id,
                Ingredient = ingredient,
                Quantity = quantity,
                Unit = unit
            };
        }
    }
}

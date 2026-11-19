using NutriWeek.GCommon.Utilities;

using NutriWeek.Data.Repository;
using NutriWeek.Data.Repository.Interfaces;

using NutriWeek.Services.Core;
using NutriWeek.Services.Core.Implementations;
using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Services.Core.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            return services
                .AddScoped<BasicModelValidator>()

                .AddScoped<IDishRepository, DishRepository>()
                .AddScoped<IMealRepository, MealRepository>()
                .AddScoped<IDailyMenuRepository, DailyMenuRepository>()
                .AddScoped<IWeeklyMenuRepository, WeeklyMenuRepository>()

                .AddScoped<IDishService, DishService>()
                .AddScoped<IMealService, MealService>()
                .AddScoped<IDailyMenuService, DailyMenuService>()
                .AddScoped<IWeeklyMenuService, WeeklyMenuService>()

                .AddScoped<IIngredientRepository, IngredientRepository>()
                .AddScoped<IPantryItemRepository, PantryItemRepository>()
                .AddScoped<IDishIngredientRepository, DishIngredientRepository>()

                .AddScoped<IPantryService, PantryService>();
        }
    }
}
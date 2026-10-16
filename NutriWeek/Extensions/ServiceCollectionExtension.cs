using NutriWeek.Services.Core.Interfaces;
using NutriWeek.Services.Core.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IDishService, DishService>();
        }
    }
}

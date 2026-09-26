using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configurations
{
    public class WeeklyMenuConfiguration
    {
        public readonly IEnumerable<WeeklyMenu> seedWeeklyMenus = new List<WeeklyMenu>
        {

          new WeeklyMenu
          {
              Id = 1,
              Title = "Healthy Week",
              WeekStartDate = new DateTime(2026, 9, 21)
          }

        };
        public void Configure(EntityTypeBuilder<WeeklyMenu> builder)
        {
            builder.HasData(seedWeeklyMenus);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configurations
{
    public class DailyMenuConfiguration : IEntityTypeConfiguration<DailyMenu>
    {
        public readonly IEnumerable<DailyMenu> seedDailyMenus = new List<DailyMenu>
        {
                new DailyMenu
                {
                    Id = 1,
                    Date = new DateTime(2026, 9, 21),
                    WeeklyMenuId = 1
                },
                new DailyMenu
                {
                    Id = 2,
                    Date = new DateTime(2026, 9, 22),
                    WeeklyMenuId = 1
                }
        };

        public void Configure(EntityTypeBuilder<DailyMenu> builder)
        {
            builder.HasData(seedDailyMenus);
        }
    }
}

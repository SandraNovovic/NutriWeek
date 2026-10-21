using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configurations
{
    public class DailyMenuConfiguration: IEntityTypeConfiguration<DailyMenu>
    {
        public void Configure(EntityTypeBuilder<DailyMenu> builder)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configurations
{
    public class WeeklyMenuConfiguration:IEntityTypeConfiguration<WeeklyMenu>
    {
        public void Configure(EntityTypeBuilder<WeeklyMenu> builder)
        {

        }
    }
}

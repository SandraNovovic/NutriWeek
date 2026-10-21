using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriWeek.Data.Models;
using NutriWeek.Data.Models.Enums;

    namespace NutriWeek.Data.Configurations
    {
        public class MealConfiguration : IEntityTypeConfiguration<Meal>
        {
            public void Configure(EntityTypeBuilder<Meal> builder)
            {

            }
        }
    }


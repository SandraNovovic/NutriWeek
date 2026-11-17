using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configuration
{
    public class IngredientConfiguration
        : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure( EntityTypeBuilder<Ingredient> builder)
        {
            builder
                .Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);


            builder
                .HasIndex(i => i.Name)
                .IsUnique();
        }
    }
}
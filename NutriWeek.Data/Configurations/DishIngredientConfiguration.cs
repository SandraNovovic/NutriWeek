using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configuration
{
    public class DishIngredientConfiguration
        : IEntityTypeConfiguration<DishIngredient>
    {
        public void Configure(
            EntityTypeBuilder<DishIngredient> builder)
        {
            builder
                .Property(di => di.Quantity)
                .HasPrecision(10, 2);


            builder
                .HasIndex(di => new
                {
                    di.DishId,
                    di.IngredientId
                })
                .IsUnique();


            builder
                .HasOne(di => di.Dish)
                .WithMany(d => d.DishIngredients)
                .HasForeignKey(di => di.DishId)
                .OnDelete(DeleteBehavior.Cascade);


            builder
                .HasOne(di => di.Ingredient)
                .WithMany(i => i.DishIngredients)
                .HasForeignKey(di => di.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
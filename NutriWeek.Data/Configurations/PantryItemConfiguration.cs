using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using NutriWeek.Data.Models;

namespace NutriWeek.Data.Configuration
{
    public class PantryItemConfiguration
        : IEntityTypeConfiguration<PantryItem>
    {
        public void Configure(
            EntityTypeBuilder<PantryItem> builder)
        {
            builder
                .Property(p => p.UserId)
                .IsRequired();


            builder
                .Property(p => p.Quantity)
                .HasPrecision(10, 2);


            builder
                .HasIndex(p => new
                {
                    p.UserId,
                    p.IngredientId
                })
                .IsUnique();


            builder
                .HasOne(p => p.Ingredient)
                .WithMany(i => i.PantryItems)
                .HasForeignKey(p => p.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using NutriWeek.Data.Models;
using NutriWeek.Data.Repository.Common;
using NutriWeek.Data.Repository.Interfaces;

namespace NutriWeek.Data.Repository
{
    public class IngredientRepository
        : BaseRepository<Ingredient>,
          IIngredientRepository
    {
        public IngredientRepository(
            NutriWeekDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
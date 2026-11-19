using NutriWeek.Data.Models;
using NutriWeek.Data.Repository.Common;
using NutriWeek.Data.Repository.Interfaces;

namespace NutriWeek.Data.Repository
{
    public class DishIngredientRepository
        : BaseRepository<DishIngredient>,
          IDishIngredientRepository
    {
        public DishIngredientRepository(
            NutriWeekDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
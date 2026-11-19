using NutriWeek.Data.Models;
using NutriWeek.Data.Repository.Common;
using NutriWeek.Data.Repository.Interfaces;

namespace NutriWeek.Data.Repository
{
    public class PantryItemRepository
        : BaseRepository<PantryItem>,
          IPantryItemRepository
    {
        public PantryItemRepository(NutriWeekDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
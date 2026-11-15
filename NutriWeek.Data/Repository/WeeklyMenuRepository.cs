using CinemaApp.Data.Repository.Common;
using NutriWeek.Data.Models;
using NutriWeek.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Data.Repository
{
    public class WeeklyMenuRepository:BaseSoftDeleteableRepository<WeeklyMenu>,IWeeklyMenuRepository
    {
        public WeeklyMenuRepository(NutriWeekDbContext dbContext)
            :base(dbContext)
        {


            
        }
    }
}

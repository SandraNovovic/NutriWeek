using NutriWeek.GCommon.Models;
using NutriWeek.Data.Repository.Interfaces.Common;
using NutriWeek.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriWeek.Data.Repository.Interfaces
{
    public interface IDailyMenuRepository: ISoftDeleteableRepository<DailyMenu>
    {
    }
}

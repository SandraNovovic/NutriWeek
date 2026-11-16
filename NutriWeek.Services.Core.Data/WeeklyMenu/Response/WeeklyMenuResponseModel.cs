using NutriWeek.Services.Core.Data.DailyMenu.Response;

namespace NutriWeek.Services.Core.Data.WeeklyMenu.Response
{
    public class WeeklyMenuResponseModel
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public int TotalCalories { get; set; }

        public int TotalPreparationTime { get; set; }

        public int TotalMeals { get; set; }

        public int TotalPortions { get; set; }

        public List<GetAllDailyMenusResponseModel> DailyMenus { get; set; }
            = new List<GetAllDailyMenusResponseModel>();
    }
}
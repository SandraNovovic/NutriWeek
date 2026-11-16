namespace NutriWeek.Services.Core.Data.DailyMenu.Response
{
    public class GetAllDailyMenusResponseModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Calories { get; set; }

        public int PreparationTime { get; set; }

        public int TotalMeals { get; set; }

        public int Portions { get; set; }

        public List<DailyMenuMealResponseModel> BreakfastMeals { get; set; }
            = new List<DailyMenuMealResponseModel>();

        public List<DailyMenuMealResponseModel> LunchMeals { get; set; }
            = new List<DailyMenuMealResponseModel>();

        public List<DailyMenuMealResponseModel> DinnerMeals { get; set; }
            = new List<DailyMenuMealResponseModel>();
    }
}
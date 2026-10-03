namespace NutriWeek.ViewModels.DailyMenus
{
    public class DailyMenuIndexViewModel
    {
        public DateTime Date { get; set; }

        public int Calories { get; set; }   

        public int PreparationTime { get; set; }    

        public int TotalMeals { get; set; }

        public int Portions { get; set; }

        public IEnumerable<DailyMenuMealViewModel> BreakfastMeals { get; set; } = new List<DailyMenuMealViewModel>();

        public IEnumerable<DailyMenuMealViewModel> LunchMeals { get; set; } = new List<DailyMenuMealViewModel>();

        public IEnumerable<DailyMenuMealViewModel> DinnerMeals { get; set; } = new List<DailyMenuMealViewModel>();
    }
}

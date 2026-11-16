namespace NutriWeek.Services.Core.Data.Meal.Response
{
    public class MealServiceStatusResponseModel
    {
        public bool Status { get; set; }

        public IEnumerable<string?> ErrorMessages { get; set; }
            = new List<string?>();
    }
}
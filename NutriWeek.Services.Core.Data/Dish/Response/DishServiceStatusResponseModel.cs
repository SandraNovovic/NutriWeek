namespace NutriWeek.Services.Core.Data.Dish.Response
{
    public class DishServiceStatusResponseModel
    {
        public bool Status { get; set; }

        public IEnumerable<string?> ErrorMessages { get; set; }
            = new List<string?>();
    }
}
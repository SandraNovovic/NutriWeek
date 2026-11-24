namespace NutriWeek.Services.Core.Data.Dish.Response
{
    public class GetDishesPageResponseModel
    {
        public List<GetAllDishesResponseModel> Dishes { get; set; }
            = new List<GetAllDishesResponseModel>();

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int VegetarianCount { get; set; }

        public int VeganCount { get; set; }

        public int MeatCount { get; set; }

        public int FishCount { get; set; }
    }
}

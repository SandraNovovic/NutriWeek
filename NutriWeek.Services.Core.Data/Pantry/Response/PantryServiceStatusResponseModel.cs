namespace NutriWeek.Services.Core.Data.Pantry.Response
{
    public class PantryServiceStatusResponseModel
    {
        public bool Status { get; set; }

        public IEnumerable<string?> ErrorMessages { get; set; }
            = Enumerable.Empty<string?>();
    }
}
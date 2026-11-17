namespace NutriWeek.Data.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;


        public ICollection<DishIngredient> DishIngredients { get; set; }
            = new HashSet<DishIngredient>();


        public ICollection<PantryItem> PantryItems { get; set; }
            = new HashSet<PantryItem>();
    }
}
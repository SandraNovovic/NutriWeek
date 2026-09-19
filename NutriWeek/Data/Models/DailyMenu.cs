using System.ComponentModel.DataAnnotations;

namespace NutriWeek.Data.Models
{
    public class DailyMenu
    {
        [Key]
        public int Id { get; set; }


        public DateTime Date { get; set; }

        public int WeeklyMenuId { get; set; }

        public WeeklyMenu WeeklyMenu { get; set; } = null!;

        public ICollection<Meal> Meals { get; set; } = new HashSet<Meal>();
    }
}

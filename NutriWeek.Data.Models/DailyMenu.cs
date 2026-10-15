using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NutriWeek.Data.Models
{
    public class DailyMenu
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey(nameof(WeeklyMenu))]
        public int WeeklyMenuId { get; set; }

        public WeeklyMenu WeeklyMenu { get; set; } = null!;

        public ICollection<Meal> Meals { get; set; } = new HashSet<Meal>();
    }
}

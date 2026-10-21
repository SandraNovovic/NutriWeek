using System.ComponentModel.DataAnnotations;


namespace NutriWeek.Data.Models
{
    public class WeeklyMenu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public DateTime WeekStartDate { get; set; }

        public string? UserId { get; set; }
        public ICollection<DailyMenu> DailyMenus { get; set; } =
            new HashSet<DailyMenu>();
    }
}

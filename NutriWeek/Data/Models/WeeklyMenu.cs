using System.ComponentModel.DataAnnotations;
using static NutriWeek.Data.EntityValidations;
namespace NutriWeek.Data.Models
{
    public class WeeklyMenu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TitleMaxLenght)]
        public string Title { get; set; } = null!;

        public DateTime WeekStartDate { get; set; }

        public ICollection<DailyMenu> DailyMenus { get; set; } = new HashSet<DailyMenu>();
    }
}

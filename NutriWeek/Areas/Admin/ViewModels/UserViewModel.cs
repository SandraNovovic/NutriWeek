namespace NutriWeek.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public List<string> Roles { get; set; }
            = new List<string>();

        public List<string> AvailableRoles { get; set; }
            = new List<string>();
    }
}

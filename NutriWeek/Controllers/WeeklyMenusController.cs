using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Controllers
{
    public class WeeklyMenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

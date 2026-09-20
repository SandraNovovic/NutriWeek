using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Controllers
{
    public class DailyMenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

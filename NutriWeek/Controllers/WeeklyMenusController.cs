using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Web.Controllers
{
    public class WeeklyMenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

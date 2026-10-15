using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Web.Controllers
{
    public class DailyMenusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

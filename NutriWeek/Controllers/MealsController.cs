using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Controllers
{
    public class MealsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

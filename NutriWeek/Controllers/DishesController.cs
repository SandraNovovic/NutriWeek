using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Controllers
{
    public class DishesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

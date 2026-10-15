using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Web.Controllers
{
    public class DishesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

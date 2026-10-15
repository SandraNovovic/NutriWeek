using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Web.Controllers
{
    public class MealsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

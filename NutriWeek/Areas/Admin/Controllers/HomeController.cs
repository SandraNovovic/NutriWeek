using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static NutriWeek.GCommon.ApplicationConstants;

namespace NutriWeek.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AdministratorRoleName)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

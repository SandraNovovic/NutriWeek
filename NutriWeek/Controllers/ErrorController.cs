using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NutriWeek.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [Route("Error/StatusCode/{statusCode:int}")]
        public IActionResult StatusCodePage(
            int statusCode)
        {
            Response.StatusCode =
                statusCode;


            if (statusCode == 404)
            {
                return View(
                    "NotFound");
            }


            return View(
                "ServerError");
        }


        [Route("Error/500")]
        public IActionResult ServerError()
        {
            Response.StatusCode =
                500;


            return View(
                "ServerError");
        }
    }
}

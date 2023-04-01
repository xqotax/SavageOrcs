using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Web.ViewModels.Error;

namespace SavageOrcs.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error(string val)
        {
            var errorViewModel = new ErrorViewModel
            {
                RouteName = val
            };
            return View(errorViewModel);
        }

        [Route("Error/ServerError")]
        public IActionResult ServerError()
        {
            return View();
        }

        public IActionResult NotFound(string info)
        {
            Response.StatusCode = 404;
            return View(model: info);
        }
    }
}

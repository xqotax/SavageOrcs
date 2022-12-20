using Microsoft.AspNetCore.Mvc;

namespace SavageOrcs.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound(string info)
        {
            Response.StatusCode = 404;
            return View(model: info);
        }
    }
}

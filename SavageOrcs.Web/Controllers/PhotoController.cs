using Microsoft.AspNetCore.Mvc;

namespace SavageOrcs.Web.Controllers
{
    public class PhotoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

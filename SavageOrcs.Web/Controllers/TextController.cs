using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SavageOrcs.Web.Controllers
{
    public class TextController : Controller
    {
        private readonly ILogger<TextController> _logger;

        public TextController(ILogger<TextController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Catalogue()
        {
            return View();
        }
    }
}

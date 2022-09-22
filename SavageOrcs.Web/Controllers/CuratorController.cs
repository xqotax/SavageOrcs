using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SavageOrcs.Web.Controllers
{
    public class CuratorController : Controller
    {
        private readonly ILogger<CuratorController> _logger;

        public CuratorController(ILogger<CuratorController> logger)
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

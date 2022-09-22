using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Web.ViewModels.Map;

namespace SavageOrcs.Web.Controllers
{
    public class MapController : Controller
    {
        private readonly ILogger<MapController> _logger;

        public MapController(ILogger<MapController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Main(string lat = "48.5819022", string lng = "32.0356408", string zoom = "6")
        {
            var mapCoordinatesViewModel = new MapCoordinatesViewModel
            {
                Lat = lat,
                Lng = lng,
                Zoom = zoom
            };

            return View(mapCoordinatesViewModel);
        }
    }
}

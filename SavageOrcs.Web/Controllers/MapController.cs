using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.BusinessObjects;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Map;

namespace SavageOrcs.Web.Controllers
{
    public class MapController : Controller
    {
        private readonly ILogger<MapController> _logger;
        private readonly IMapService _mapService;

        public MapController(ILogger<MapController> logger, IMapService mapService)
        {
            _logger = logger;
            _mapService = mapService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Main(string lat = "48.5819022", string lng = "32.0356408", string zoom = "6")
        {
            var mapDto = await _mapService.GetMap(1);

            var mapCoordinatesViewModel = new MapCoordinatesViewModel
            {
                Lat = lat,
                Lng = lng,
                Zoom = zoom,
                Name = mapDto.Name,
                Id = 1,
                MapMarkViewModels = mapDto.MapMarkDtos.Select(x => new MapMarkViewModel { 
                    Id = x.Id,
                    Lat = x.Lat?.ToString().Replace(',', '.'),
                    Lng = x.Lng?.ToString().Replace(',', '.'),
                    Name = x.Name,
                    IsApproximate = x.IsApproximate is not null && x.IsApproximate.Value
                }).ToArray(),
                MapClusterViewModels = mapDto.MapClusterDtos.Select(x => new MapClusterViewModel
                {
                    Id = x.Id,
                    Lat = x.Lat?.ToString().Replace(',', '.'),
                    Lng = x.Lng?.ToString().Replace(',', '.'),
                    Name = x.Name
                }).ToArray()

            };

            return View(mapCoordinatesViewModel);
        }
    }
}

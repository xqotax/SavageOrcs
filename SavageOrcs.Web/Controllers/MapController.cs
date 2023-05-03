using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Map;
using SavageOrcs.Web.ViewModels.Mark;

namespace SavageOrcs.Web.Controllers
{
    public class MapController : Controller
    {
        private readonly ILogger<MapController> _logger;
        private readonly IMapService _mapService;
        private readonly IAreaService _areaService;
        private readonly IMarkService _markService;
        private readonly IClusterService _clusterService;
        private readonly IHelperService _helperService;
        private readonly IConfiguration _configuration;

        public MapController(ILogger<MapController> logger, IMapService mapService, IHelperService helperService, IClusterService clusterService, IMarkService markService, IAreaService areaService, IConfiguration configuration)
        {
            _logger = logger;
            _mapService = mapService;
            _helperService = helperService;
            _clusterService = clusterService;
            _markService = markService;
            _areaService = areaService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult ChangeLanguage(string culture)
        {
            //var a = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Get18PlusPopup()
        {
            return PartialView("_18PlusPopup");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Main(string lat = "48.5819022", string lng = "32.0356408", string zoom = "6")
        {
            ViewBag.GoogleSiteVerificationContent = _configuration.GetSection("GoogleSiteVerificationContent").Value;

            var mapDto = await _mapService.GetMap(1);

            var mapCoordinatesViewModel = new MapCoordinatesViewModel
            {
                Lat = lat,
                Lng = lng,
                Zoom = zoom,
                Name = mapDto.Name,
                GoogleMapKey = _configuration.GetSection("GoogleMapApiKey").Value,
                Id = 1,
                MapMarkViewModels = mapDto.MapMarkDtos.Select(x => new MapMarkViewModel { 
                    Id = x.Id,
                    Lat = x.Lat?.ToString().Replace(',', '.'),
                    Lng = x.Lng?.ToString().Replace(',', '.'),
                    Name = _helperService.GetTranslation(x.Name, x.NameEng),
                }).ToArray(),
                MapClusterViewModels = mapDto.MapClusterDtos.Select(x => new MapClusterViewModel
                {
                    Id = x.Id,
                    Lat = x.Lat?.ToString().Replace(',', '.'),
                    Lng = x.Lng?.ToString().Replace(',', '.'),
                    Name = _helperService.GetTranslation(x.Name, x.NameEng)
                }).ToArray()

            };
            var markDtos = await _markService.GetShortMarks();
            var clusterDtos = await _clusterService.GetClusters();

            if (!User.IsInRole("Admin"))
            {
                markDtos = markDtos.Where(x => x.IsVisible).ToArray();
                clusterDtos = clusterDtos.Where(x => x.Marks.Length > 0 && x.Marks.Any(y => y.IsVisible)).ToArray();
            }

            mapCoordinatesViewModel.KeyWords = (await _helperService.GetAllKeyWords()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetTranslation(x.Name, x.NameEng)
            }).ToArray();
            mapCoordinatesViewModel.ClusterNames = clusterDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetSubstringForFilters(_helperService.GetTranslation(x.Name, x.NameEng))
            }).OrderBy(x => x.Name).ToArray();
            mapCoordinatesViewModel.Areas = (await _areaService.GetUsedAreasAsync()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetTranslation(x.Name, x.NameEng) + ", " 
                    + _helperService.GetTranslation(x.Community, x.CommunityEng) + ", " 
                    + _helperService.GetTranslation(x.Region, x.RegionEng),
            }).ToArray();
            mapCoordinatesViewModel.MarkNames = markDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id.Value,
                Name = _helperService.GetSubstringForFilters(_helperService.GetTranslation(x.Name, x.NameEng))
            }).Where(x => !mapCoordinatesViewModel.KeyWords
                    .Any(y => x.Name is not null && y.Name is not null && x.Name.Contains(y.Name, StringComparison.OrdinalIgnoreCase)))
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .OrderBy(x => x.Name)
                .ToArray();

            return View(mapCoordinatesViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetMarks([FromBody] UnitedCatalogueViewModel filter)
        {
            var markDtos = await _markService.GetMarksByFilters(filter.SelectedKeyWordIds, filter.SelectedMarkIds, filter.SelectedClusterIds, filter.SelectedAreaIds);
            var clusterDtos = await _clusterService.GetClustersByFilters(filter.SelectedKeyWordIds, filter.SelectedClusterIds, filter.SelectedAreaIds);

            var markCatalogueViewModels = markDtos.Select(x => new MapMarkViewModel
            {
                Id = x.Id.Value,
                Lat = x.Lat?.ToString().Replace(',', '.'),
                Lng = x.Lng?.ToString().Replace(',', '.'),
                Name = _helperService.GetTranslation(x.Name, x.NameEng),
                IsCluster = false
            }).ToArray();

            markCatalogueViewModels = markCatalogueViewModels.Concat(clusterDtos.Select(x => new MapMarkViewModel
            {
                Id = x.Id,
                Lat = x.Lat.ToString().Replace(',', '.'),
                Lng = x.Lng.ToString().Replace(',', '.'),
                Name = _helperService.GetTranslation(x.Name, x.NameEng),
                IsCluster = true
            })).ToArray();
           

            return Json(markCatalogueViewModels);
        }
    }
}

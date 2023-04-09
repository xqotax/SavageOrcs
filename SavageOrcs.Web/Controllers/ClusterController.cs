using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Cluster;
using SavageOrcs.DataTransferObjects.Curators;
using SavageOrcs.DataTransferObjects.Maps;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Cluster;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using System.Data;
using System.Globalization;

namespace SavageOrcs.Web.Controllers
{
    public class ClusterController : Controller
    {
        
        private readonly IHelperService _imageService;
        private readonly IAreaService _areaService;
        private readonly IClusterService _clusterService;
        private readonly ICuratorService _curatorService;
        private readonly IHelperService _helperService;

        public ClusterController(IAreaService areaService, IHelperService imageService, IClusterService clusterService, ICuratorService curatorService, IHelperService helperService)
        {
            _clusterService = clusterService;
            _areaService = areaService;
            _imageService = imageService;
            _curatorService = curatorService;
            _helperService = helperService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            var clusterDto = id.HasValue ? await _clusterService.GetClusterById(id.Value) : null;
            var areaDtos = Array.Empty<AreaShortDto>();

            var emptySelect = _helperService.GetEmptySelect();
            var emptySelectArr = new GuidIdAndNameViewModel[] {
                new GuidIdAndNameViewModel
                {
                    Id = emptySelect.Id,
                    Name = emptySelect.Name
                }
            };

            var curatorDtos = await _curatorService.GetCurators();


            if (clusterDto is null)
            {
                areaDtos = await _areaService.GetAreasByNameAsync("Херсон");
            }
            else
            {
                areaDtos = await _areaService.GetAreasByNameAsync(clusterDto.Area is null ? "Херсон" : clusterDto.Area.Name);
            }

            var addClusterViewModel = new AddClusterViewModel
            {
                Id = clusterDto?.Id,
                Lat = clusterDto is null ? "48.6125528" : clusterDto.Lat.ToString(CultureInfo.InvariantCulture),
                Lng = clusterDto is null ? "31.0275809" : clusterDto.Lng.ToString(CultureInfo.InvariantCulture),
                Zoom = "6",
                Name = clusterDto?.Name,
                Description = clusterDto?.Description,
                DescriptionEng = clusterDto?.DescriptionEng,
                ResourceName = clusterDto?.ResourceName,    
                ResourceUrl = clusterDto?.ResourceUrl,
                CuratorId = clusterDto?.Curator?.Id,
                CuratorName = clusterDto?.Curator?.Name,
                AreaId = clusterDto?.Area?.Id,
                AreaName = clusterDto?.Area is null ? null : clusterDto.Area.Name + ", " + clusterDto.Area.Community + ", " + clusterDto.Area.Region,
                IsNew = !id.HasValue,
                Areas = emptySelectArr.Concat(areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                })).OrderBy(x => x.Name).ToArray(),
                Curators = emptySelectArr.Concat(curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                })).ToArray(),
                Places = (await _helperService.GetAllPlaces()).Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? x.Name : x.NameEng,
                }).ToArray(),
                SelectedPlaceIds = clusterDto is null ? Array.Empty<Guid>() : clusterDto.Places.Select(x => x.Id).ToArray()
            };

            return View(addClusterViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> Save([FromBody] SaveClusterViewModel saveClusterViewModel)
        {
            var clusterSaveDto = new ClusterSaveDto();

            var clusterSaveResult = new SaveResultClusterViewModel();
            if (saveClusterViewModel.Lat is null || saveClusterViewModel.Lng is null)
            {
                clusterSaveResult.LatOrLngInNullOrEmpty = false;
                clusterSaveResult.Success = false;
                return Json(clusterSaveResult);
            }

            clusterSaveDto.Description = saveClusterViewModel.Description;
            clusterSaveDto.DescriptionEng = saveClusterViewModel.DescriptionEng;
            clusterSaveDto.ResourceName = saveClusterViewModel.ResourceName;
            clusterSaveDto.ResourceNameEng = saveClusterViewModel.ResourceNameEng;
            clusterSaveDto.ResourceUrl = saveClusterViewModel.ResourceUrl;
            clusterSaveDto.PlaceIds = saveClusterViewModel.SelectedPlaceIds;
            clusterSaveDto.Name = saveClusterViewModel.Name;
            clusterSaveDto.Id = saveClusterViewModel.Id;
            clusterSaveDto.Lat = double.Parse(saveClusterViewModel.Lat, CultureInfo.InvariantCulture);
            clusterSaveDto.Lng = double.Parse(saveClusterViewModel.Lng, CultureInfo.InvariantCulture);
            clusterSaveDto.AreaId = saveClusterViewModel.AreaId;
            clusterSaveDto.CuratorId = saveClusterViewModel.CuratorId;
            clusterSaveDto.MapId = 1;

            var clusterSaveResultDto = await _clusterService.SaveCluster(clusterSaveDto);
            clusterSaveResult.Success = clusterSaveResultDto.Success;
            clusterSaveResult.Id = clusterSaveResultDto.Id;

            return Json(new SaveMarkResultViewModel
            {
                Id = clusterSaveResult.Id,
                Success = clusterSaveResult.Success,
                Url = "/Cluster/Revision/{id}",
                Text = clusterSaveResult.Success ? "Кластер успішно збережено" : "Помилочка!!!"
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id)
        {
            return RedirectToAction("Revision", "Mark", new {Id = id, isCluster = true});
        }

        [AllowAnonymous]
        //public IActionResult Catalogue()
        //{
        //    var filterCatalogueClusterViewModel = new FilterCatalogueClusterViewModel()
        //    {
        //        AreaName = "",
        //        KeyWord = "",
        //        ClusterDescription = "",
        //        ClusterName = "",
        //        MinCountOfMarks = null
        //    };

        //    return View("Catalogue", filterCatalogueClusterViewModel);
        //}

        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    var clusterDto = id.HasValue ? await _clusterService.GetClusterById(id.Value) : null;

        //    if (clusterDto is null)
        //    {
        //        return NotFound();
        //    }
        //    var areaDtos = await _areaService.GetAreasByNameAsync(clusterDto.Area is null ? "Херсон" : clusterDto.Area.Name);

        //    var addClusterViewModel = new AddClusterViewModel
        //    {
        //        Id = clusterDto?.Id,
        //        Lat = clusterDto is null ? "48.6125528" : clusterDto.Lat.ToString(CultureInfo.InvariantCulture),
        //        Lng = clusterDto is null ? "31.0275809" : clusterDto.Lng.ToString(CultureInfo.InvariantCulture),
        //        Zoom = "6",
        //        Name = clusterDto?.Name,
        //        Description = clusterDto?.Description,
        //        AreaId = clusterDto?.Area?.Id,
        //        AreaName = clusterDto?.Area?.Name,
        //        IsNew = !id.HasValue,
        //        Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
        //        {
        //            Name = x.Name + ", " + x.Community + ", " + x.Region,
        //            Id = x.Id
        //        }).OrderBy(x => x.Name).ToArray(),
        //        ToDelete = true
        //    };

        //    return View("Add", addClusterViewModel);
        //}

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCluster()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeleteConfirm(Guid id, bool withMarks)
        {
            var result = await _clusterService.DeleteCluster(id, withMarks);

            return Json(new ResultViewModel
            {
                Id = id,
                Success = result,
                Url = "/Mark/Catalogue",
                Text = result ? "Кластер успішно видалено" : "Помилка, зверніться до адміністратора"
            });
        }
    }
}

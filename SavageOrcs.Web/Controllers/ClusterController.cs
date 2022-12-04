using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Cluster;
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
        
        private readonly IImageService _imageService;
        private readonly IAreaService _areaService;
        private readonly IClusterService _clusterService;

        public ClusterController(IAreaService areaService, IImageService imageService, IClusterService clusterService)
        {
            _clusterService = clusterService;
            _areaService = areaService;
            _imageService = imageService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            var clusterDto = id.HasValue ? await _clusterService.GetClusterById(id.Value) : null;
            var areaDtos = Array.Empty<AreaShortDto>();

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
                AreaId = clusterDto?.Area?.Id,
                AreaName = clusterDto?.Area?.Name,
                IsNew = !id.HasValue,
                Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                }).OrderBy(x => x.Name).ToArray()
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
            clusterSaveDto.Name = saveClusterViewModel.Name;
            clusterSaveDto.Id = saveClusterViewModel.Id;
            clusterSaveDto.Lat = double.Parse(saveClusterViewModel.Lat, CultureInfo.InvariantCulture);
            clusterSaveDto.Lng = double.Parse(saveClusterViewModel.Lng, CultureInfo.InvariantCulture);
            clusterSaveDto.AreaId = saveClusterViewModel.AreaId;
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
            var clusterDto = await _clusterService.GetClusterById(id);
            if (clusterDto is null)
                return NotFound("Скупчення не знайдено");

            var clusterRevisionViewNodel = new RevisionClusterViewModel
            {
                Id = clusterDto.Id,
                Name = clusterDto.Name,
                Description = clusterDto.Description,
                AreaId = clusterDto.Area?.Id,
                AreaName = clusterDto.Area is null ? null: clusterDto.Area.Name + ", " + clusterDto.Area.Community + ", " + clusterDto.Area.Region,
                Lat =  clusterDto.Lat.ToString(CultureInfo.InvariantCulture),
                Lng = clusterDto.Lng.ToString(CultureInfo.InvariantCulture),
                Marks = clusterDto.Marks?.Select(x => new RevisionClusterMarksViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description?.Replace('\n', ' '),
                    DescriptionEng = x.DescriptionEng?.Replace('\n', ' ').Replace('"', '\''),
                    ResourceUrl = x.ResourceUrl,
                    Image = x.Image is null ? null : _imageService.GetImage(x.Image),
                    Area = x.Area is null ? null : new GuidIdAndStringName
                    {
                        Id = x.Area.Id,
                        Name = x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region,
                    },
                }).ToArray()
            };

            return View(clusterRevisionViewNodel);
        }

        [AllowAnonymous]
        public IActionResult Catalogue()
        {
            var filterCatalogueClusterViewModel = new FilterCatalogueClusterViewModel()
            {
                AreaName = "",
                KeyWord = "",
                ClusterDescription = "",
                ClusterName = "",
                MinCountOfMarks = null
            };

            return View("Catalogue", filterCatalogueClusterViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetClusters([FromBody] FilterCatalogueClusterViewModel filters)
        {
            var clusterDtos = await _clusterService.GetClustersByFilters(filters.KeyWord, filters.ClusterName, filters.ClusterDescription, filters.AreaName, filters.MinCountOfMarks);

            var clusterViewModels = clusterDtos.Select(x => new CatalogueClusterViewModel { 
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Area = x.Area is null? null : new GuidIdAndNameViewModel
                {
                    Id = x.Area.Id,
                    Name = x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region,
                },
                MarkCount = x.Marks is null ? 0 : x.Marks.Length
            }).ToArray();

            return Json(clusterViewModels);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var clusterDto = id.HasValue ? await _clusterService.GetClusterById(id.Value) : null;

            if (clusterDto is null)
            {
                return NotFound();
            }
            var areaDtos = await _areaService.GetAreasByNameAsync(clusterDto.Area is null ? "Херсон" : clusterDto.Area.Name);

            var addClusterViewModel = new AddClusterViewModel
            {
                Id = clusterDto?.Id,
                Lat = clusterDto is null ? "48.6125528" : clusterDto.Lat.ToString(CultureInfo.InvariantCulture),
                Lng = clusterDto is null ? "31.0275809" : clusterDto.Lng.ToString(CultureInfo.InvariantCulture),
                Zoom = "6",
                Name = clusterDto?.Name,
                Description = clusterDto?.Description,
                AreaId = clusterDto?.Area?.Id,
                AreaName = clusterDto?.Area?.Name,
                IsNew = !id.HasValue,
                Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                }).OrderBy(x => x.Name).ToArray(),
                ToDelete = true
            };

            return View("Add", addClusterViewModel);
        }

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
                Url = "/Cluster/Catalogue",
                Text = result ? "Кластер успішно видалено" : "Помилка, зверніться до адміністратора"
            });
        }
    }
}

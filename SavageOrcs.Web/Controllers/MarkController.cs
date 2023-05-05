using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.BusinessObjects;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using System.Globalization;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using Microsoft.AspNetCore.Localization;
using SavageOrcs.DataTransferObjects.Maps;
using SavageOrcs.DataTransferObjects.Cluster;
using SavageOrcs.DbContext.Migrations;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Drawing.Printing;
using SavageOrcs.DataTransferObjects.Texts;
using System.Xml.Linq;
using NuGet.Packaging;
using Microsoft.VisualBasic;

namespace SavageOrcs.Web.Controllers
{
    public class MarkController : Controller
    {
        private readonly IAreaService _areaService;
        private readonly IMarkService _markService;
        private readonly IClusterService _clusterService;
        private readonly ICuratorService _curatorService;
        private readonly IHelperService _helperService;
        private readonly IConfiguration _configuration;

        private readonly UserManager<User> _userManager;

        public MarkController(UserManager<User> userManager, IAreaService areaService, IMarkService markService, IClusterService clusterService, IHelperService helperService, ICuratorService curatorService, IConfiguration configuration)
        {
            _userManager = userManager;
            _areaService = areaService;
            _markService = markService;
            _clusterService = clusterService;
            _helperService = helperService;
            _curatorService = curatorService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id, bool isCluster = false)
        {
            if (isCluster)
            {
                var clusterDto = await _clusterService.GetClusterById(id, true);
                if (clusterDto is null)
                    return RedirectToAction("Error");

                var revisionMarkViewModel = new RevisionMarkViewModel
                {
                    Id = clusterDto.Id,
                    Name = _helperService.GetTranslation(clusterDto.Name, clusterDto.NameEng),
                    Description = _helperService.GetTranslation(clusterDto.Description, clusterDto.DescriptionEng),
                    CuratorName = _helperService.GetTranslation(clusterDto.Curator?.Name, clusterDto.Curator?.NameEng),
                    Area = clusterDto.Area is null ? "" : clusterDto.Area.Name + ", " + clusterDto.Area.Community + ", " + clusterDto.Area.Region,
                    Images = User.IsInRole("Admin") ? clusterDto.Marks.SelectMany(x => x.Images).Select(x => _helperService.GetImage(x.Content)).ToArray() :
                        clusterDto.Marks.SelectMany(x => x.Images).Where(x => x.IsVisible).Select(x => _helperService.GetImage(x.Content)).ToArray(),
                    ClusterId = null,
                    IsCluster = true,
                    ResourceUrl = clusterDto.ResourceUrl,
                    ResourceName = _helperService.GetTranslation(clusterDto.ResourceName, clusterDto.ResourceNameEng)
                };
                return View(revisionMarkViewModel);
            }

            var markDto = await _markService.GetMarkById(id);
            if (markDto is null || (!User.IsInRole("Admin") && !markDto.Images.Any(x => x.IsVisible)))
            {
                return RedirectToAction("NotFound", "Error", new { info = "Mark" });
            }
            else
            {
                var revisionMarkViewModel = new RevisionMarkViewModel
                {
                    Id = markDto.Id,
                    Name = _helperService.GetTranslation(markDto.Name, markDto.NameEng),
                    Description = _helperService.GetTranslation(markDto.Description, markDto.DescriptionEng),
                    ResourceUrl = markDto.ResourceUrl,
                    ResourceName = _helperService.GetTranslation(markDto.ResourceName, markDto.ResourceNameEng),
                    CuratorName = _helperService.GetTranslation(markDto.Curator?.Name, markDto.Curator?.NameEng),
                    Area = markDto.Area is null ? "" : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                    ClusterId = markDto.Cluster?.Id,
                    ClusterName = _helperService.GetTranslation(markDto.Cluster?.Name, markDto.Cluster?.NameEng),
                    IsCluster = false
                };

                if (User.IsInRole("Admin"))
                    revisionMarkViewModel.Images = markDto.Images.Select(x => _helperService.GetImage(x.Content)).ToArray();
                else
                    revisionMarkViewModel.Images = markDto.Images.Where(x => x.IsVisible).Select(x => _helperService.GetImage(x.Content)).ToArray();

                return View(revisionMarkViewModel);

            }
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            MarkDto? markDto = null;
            var emptySelect = _helperService.GetEmptySelect();
            var emptySelectArr = new GuidIdAndNameViewModel[] {
                new GuidIdAndNameViewModel
                {
                    Id = emptySelect.Id,
                    Name = emptySelect.Name
                }
            };

            if (id is not null)
            {
                markDto = await _markService.GetMarkById(id.Value);
            }

            var areaDtos = Array.Empty<AreaShortDto>();

            var clusterDtos = await _clusterService.GetClusters();
            var curatorDtos = await _curatorService.GetCurators();

            if (markDto is null)
            {
                areaDtos = await _areaService.GetAreasByNameAsync("Бахмут");
            }
            else
            {
                areaDtos = await _areaService.GetAreasByNameAsync(markDto.Area is null ? "Бахмут" : markDto.Area.Name);
            }

            var addMarkViewModel = new AddMarkViewModel()
            {
                Id = markDto?.Id,
                Lat = markDto?.Lat is null ? "48.6125528" : markDto.Lat.Value.ToString(CultureInfo.InvariantCulture),
                Lng = markDto?.Lng is null ? "31.0275809" : markDto.Lng.Value.ToString(CultureInfo.InvariantCulture),
                Zoom = markDto is null ? "6" : "9",
                AreaId = markDto?.Area?.Id,
                AreaName = markDto?.Area is null ? null : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                ClusterId = markDto?.Cluster?.Id,
                CuratorId = markDto?.Curator?.Id,
                ClusterName = markDto?.Cluster?.Name,
                CuratorName = markDto?.Curator?.Name,
                Description = markDto?.Description,
                DescriptionEng = markDto?.DescriptionEng,
                ResourceUrl = markDto?.ResourceUrl,
                ResourceName = markDto?.ResourceName,
                ResourceNameEng = markDto?.ResourceNameEng,
                Name = markDto?.Name,
                NameEng = markDto?.NameEng,
                Images = markDto is null ? Array.Empty<StringAndBoolViewModel>() : markDto.Images.Select(x => new StringAndBoolViewModel
                {

                    Content = _helperService.GetImage(x.Content),
                    IsVisible = x.IsVisible
                }).ToArray(),
                IsNew = markDto is null,
                Areas = emptySelectArr.Concat(areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                })).OrderBy(x => x.Name).ToArray(),
                Clusters = emptySelectArr.Concat(clusterDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })).ToArray(),
                Curators = emptySelectArr.Concat(curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                })).ToArray(),
                GoogleMapKey = _configuration.GetSection("GoogleMapApiKey").Value
            };

            return View(addMarkViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetAreas([FromBody] SearchAreasViewModel searcAreasViewModel)
        {
            var areaDtos = await _areaService.GetAreasByNameAsync(searcAreasViewModel.Text.ToUpper());
            var areaDropDownList = areaDtos.Select(x => new GuidIdAndNameViewModel
            {
                Name = x.Name + ", " + x.Community + ", " + x.Region,
                Id = x.Id
            }).OrderBy(x => x.Name).ToList();

            return Json(areaDropDownList);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveMark([FromBody] AddMarkViewModel saveMarkViewModel)
        {
            var markSaveDto = new MarkSaveDto();
            try
            {
                markSaveDto.Id = saveMarkViewModel.Id;
                markSaveDto.AreaId = saveMarkViewModel.AreaId;
                markSaveDto.ClusterId = saveMarkViewModel.ClusterId;
                markSaveDto.CuratorId = saveMarkViewModel.CuratorId;

                ClusterDto? clusterDto = null;
                if (saveMarkViewModel.ClusterId is not null && saveMarkViewModel.ClusterId != _Constants.EmptySelect)
                {
                    clusterDto = await _clusterService.GetClusterById(saveMarkViewModel.ClusterId.Value);

                    if (clusterDto is null)
                        return Json(new SaveMarkResultViewModel
                        {
                            Id = null,
                            Success = false,
                            Url = "/Mark/Revision/{id}",
                            Text = "Вибраного скупчення не існує"
                        });
                }

                if ((saveMarkViewModel.Lng is null || saveMarkViewModel.Lat is null) && (clusterDto is null))
                    return Json(new SaveMarkResultViewModel
                    {
                        Id = null,
                        Success = false,
                        Url = "",
                        Text = "Виставіть координати мітці або виберіть скупчення для неї"
                    });


                if (saveMarkViewModel.Lng is not null)
                    markSaveDto.Lng = double.Parse(saveMarkViewModel.Lng, CultureInfo.InvariantCulture);
                else if (clusterDto is not null)
                    markSaveDto.Lng = clusterDto.Lng;

                if (saveMarkViewModel.Lat is not null)
                    markSaveDto.Lat = double.Parse(saveMarkViewModel.Lat, CultureInfo.InvariantCulture);
                else if (clusterDto is not null)
                    markSaveDto.Lat = clusterDto.Lat;

                markSaveDto.Name = saveMarkViewModel.Name;
                markSaveDto.NameEng = saveMarkViewModel.NameEng;
                markSaveDto.Description = saveMarkViewModel.Description;
                markSaveDto.DescriptionEng = saveMarkViewModel.DescriptionEng;
                markSaveDto.ResourceUrl = saveMarkViewModel.ResourceUrl;
                markSaveDto.ResourceName = saveMarkViewModel.ResourceName;
                markSaveDto.ResourceNameEng = saveMarkViewModel.ResourceNameEng;
                markSaveDto.MapId = 1;
                markSaveDto.Images = saveMarkViewModel.Images.Select(x => new ByteContentAndBooIsVisible
                {
                    Content = _helperService.GetBytes(x.Content),
                    IsVisible = x.IsVisible
                }).ToArray();

                var markSaveResultDto = await _markService.SaveMark(markSaveDto);

                return Json(new SaveMarkResultViewModel
                {
                    Id = markSaveResultDto.Id,
                    Success = markSaveResultDto.Success,
                    Url = "/Mark/Revision/{id}",
                    Text = "Мітка успішно збережена"
                });
            }
            catch
            {
                return Json(new SaveMarkResultViewModel
                {
                    Id = null,
                    Success = false,
                    Url = "/Mark/Revision/{id}",
                    Text = "Мітка успішно збережена"
                });
            }


        }

        [AllowAnonymous]
        public async Task<IActionResult> Catalogue(Guid? clusterId)
        {
            var unitedCatalogueViewModel = new UnitedCatalogueViewModel();

            var markDtos = await _markService.GetShortMarks();
            var clusterDtos = await _clusterService.GetClusters();

            if (!User.IsInRole("Admin"))
            {
                markDtos = markDtos.Where(x => x.IsVisible).ToArray();
                clusterDtos = clusterDtos.Where(x => x.Marks.Length > 0 && x.Marks.Any(y => y.IsVisible)).ToArray();
            }

            unitedCatalogueViewModel.KeyWords = (await _helperService.GetAllKeyWords()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetTranslation(x.Name, x.NameEng)
            }).ToArray();
            unitedCatalogueViewModel.ClusterNames = clusterDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetSubstringForFilters(_helperService.GetTranslation(x.Name, x.NameEng))
            }).OrderBy(x => x.Name).ToArray();

            unitedCatalogueViewModel.MarkNames = markDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id.Value,
                Name = _helperService.GetSubstringForFilters(_helperService.GetTranslation(x.Name, x.NameEng))
            }).Where(x => !unitedCatalogueViewModel.KeyWords
                    .Any(y => x.Name is not null && y.Name is not null && x.Name.Contains(y.Name, StringComparison.OrdinalIgnoreCase)))
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .OrderBy(x => x.Name)
                .ToArray();

            unitedCatalogueViewModel.Areas = (await _areaService.GetUsedAreasAsync()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = _helperService.GetTranslation(x.Name, x.NameEng) + ", "
                    + _helperService.GetTranslation(x.Community, x.CommunityEng) + ", "
                    + _helperService.GetTranslation(x.Region, x.RegionEng),
            }).ToArray();

            if (clusterId is not null && (await _clusterService.GetClusterById(clusterId.Value) is not null))
            {
                var clusterDto = clusterDtos.FirstOrDefault(x => x.Id == clusterId.Value);
                if (clusterDto is null)
                    return RedirectToAction("Error");
                unitedCatalogueViewModel.Marks = clusterDto.Marks.Select(x => new MarkCatalogueViewModel
                {
                    Id = x.Id,
                    Name = _helperService.GetTranslation(x.Name, x.NameEng),
                    CuratorName = x.CuratorName,
                    ClusterName = _helperService.GetTranslation(clusterDto.Name, clusterDto.NameEng),
                    ResourceName = _helperService.GetTranslation(x.ResourceName, x.ResourceNameEng),
                    ResourceUrl = x.ResourceUrl,
                    Area = x.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel
                    {
                        Id = x.Area.Id,
                        Name = _helperService.GetTranslation(x.Area.Name, x.Area.NameEng) + ", "
                            + _helperService.GetTranslation(x.Area.Community, x.Area.CommunityEng) + ", "
                            + _helperService.GetTranslation(x.Area.Region, x.Area.RegionEng),
                    },

                }).ToArray();
            }
            else
            {
                unitedCatalogueViewModel.Marks = markDtos.Select(x => new MarkCatalogueViewModel
                {
                    Id = x.Id,
                    Name = _helperService.GetTranslation(x.Name, x.NameEng),
                    CuratorName = x.CuratorName,
                    ResourceName = _helperService.GetTranslation(x.ResourceName, x.ResourceNameEng),
                    ResourceUrl = x.ResourceUrl,
                    ClusterName = x.ClusterName,
                    Area = x.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel
                    {
                        Id = x.Area.Id,
                        Name = _helperService.GetTranslation(x.Area.Name, x.Area.NameEng) + ", "
                            + _helperService.GetTranslation(x.Area.Community, x.Area.CommunityEng) + ", "
                            + _helperService.GetTranslation(x.Area.Region, x.Area.RegionEng),
                    },
                })
                    .Concat(clusterDtos.Select(x => new MarkCatalogueViewModel
                    {
                        Id = x.Id,
                        IsCluster = true,
                        Name = _helperService.GetTranslation(x.Name, x.NameEng),
                        ResourceName = _helperService.GetTranslation(x.ResourceName, x.ResourceNameEng),
                        ResourceUrl = x.ResourceUrl,
                        CuratorName = x.Curator?.Name,
                        Area = x.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel
                        {
                            Id = x.Area.Id,
                            Name = _helperService.GetTranslation(x.Area.Name, x.Area.NameEng) + ", "
                                + _helperService.GetTranslation(x.Area.Community, x.Area.CommunityEng) + ", "
                                + _helperService.GetTranslation(x.Area.Region, x.Area.RegionEng),
                        },
                    }))
                    .OrderByDescending(x => x.Name)
                    .ToArray();

            }

            return View("Catalogue", unitedCatalogueViewModel);
        }

        public async Task<IActionResult> GetImages(Guid id, bool isCluster, int index)
        {
            var revisionImageViewModel = new RevisionImageViewModel();
            if (isCluster)
            {
                var clusterDto = await _clusterService.GetClusterById(id, true);
                revisionImageViewModel = new RevisionImageViewModel
                {
                    IsCluster = isCluster,
                    Id = clusterDto.Id,
                    Images = clusterDto.Marks.SelectMany(x => x.Images).Select(x => _helperService.GetImage(x.Content)).ToArray()
                };
                return PartialView("_CatalogueImage", revisionImageViewModel);
            }

            var markDto = await _markService.GetMarkById(id);
            revisionImageViewModel = new RevisionImageViewModel
            {
                IsCluster = isCluster,
                Id = id,
                Images = markDto.Images.Select(x => _helperService.GetImage(x.Content)).ToArray()
            };
            return PartialView("_CatalogueImage", revisionImageViewModel);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetMarks([FromBody] UnitedCatalogueViewModel filter)
        {
            var markDtos = await _markService.GetMarksByFilters(filter.SelectedKeyWordIds, filter.SelectedMarkIds, filter.SelectedClusterIds, filter.SelectedAreaIds);
            var clusterDtos = await _clusterService.GetClustersByFilters(filter.SelectedKeyWordIds, filter.SelectedClusterIds, filter.SelectedAreaIds);

            var markCatalogueViewModel = markDtos.Select(x => new MarkCatalogueViewModel
            {
                Id = x.Id,
                Name = _helperService.GetTranslation(x.Name, x.NameEng),
                CuratorName = x.CuratorName,
                ResourceName = _helperService.GetTranslation(x.ResourceName, x.ResourceNameEng),
                ResourceUrl = x.ResourceUrl,
                ClusterName = x.ClusterName,
                Area = x.Area is null ? new GuidIdAndNameViewModel() :
                    new GuidIdAndNameViewModel
                    {
                        Id = x.Area.Id,
                        Name = _helperService.GetTranslation(x.Area.Name, x.Area.NameEng) + ", "
                    + _helperService.GetTranslation(x.Area.Community, x.Area.CommunityEng) + ", "
                    + _helperService.GetTranslation(x.Area.Region, x.Area.RegionEng),
                    },
            }).ToArray();

            markCatalogueViewModel = markCatalogueViewModel.Concat(clusterDtos.Select(x => new MarkCatalogueViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CuratorName = x.Curator.Name,
                ResourceName = _helperService.GetTranslation(x.ResourceName, x.ResourceNameEng),
                ResourceUrl = x.ResourceUrl,
                IsCluster = true,
                Area = x.Area is null ? new GuidIdAndNameViewModel() :
                    new GuidIdAndNameViewModel
                    {
                        Id = x.Area.Id,
                        Name = _helperService.GetTranslation(x.Area.Name, x.Area.NameEng) + ", "
                        + _helperService.GetTranslation(x.Area.Community, x.Area.CommunityEng) + ", "
                        + _helperService.GetTranslation(x.Area.Region, x.Area.RegionEng),
                    },
            })).ToArray();

            return PartialView("_CatalogueDataRows", markCatalogueViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult NavigationAdmin()
        {
            return PartialView("~/Views/Shared/NavigationAdmin.cshtml");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteMark()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ImageToInsert([FromBody] string content)
        {
            return PartialView("_ImageToInsert", new StringAndBoolViewModel
            {
                IsVisible = true,
                Content = content
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeleteConfirm(Guid id)
        {
            var result = await _markService.DeleteMark(id);


            return Json(new ResultViewModel
            {
                Id = id,
                Success = result,
                Url = "/Mark/Catalogue",
                Text = result ? "Мітка успішно видалена" : "Помилка, зверніться до адміністратора"
            });
        }

        [AllowAnonymous]
        public IActionResult RevisionImage([FromBody] string? data)
        {
            return PartialView("_RevisionImage", data);
        }
    }
}

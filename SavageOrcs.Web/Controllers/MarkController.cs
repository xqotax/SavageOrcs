using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DbContext;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using System.Globalization;
using SavageOrcs.DataTransferObjects.Marks;
using System.Text;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using System.Linq;
using Microsoft.AspNetCore.Localization;
using SavageOrcs.DataTransferObjects.Maps;
using SavageOrcs.DataTransferObjects.Cluster;

namespace SavageOrcs.Web.Controllers
{
    public class MarkController : Controller
    {
        private readonly IAreaService _areaService;
        private readonly IMarkService _markService;
        private readonly IKeyWordService _keyWordService;
        private readonly IClusterService _clusterService;
        private readonly ICuratorService _curatorService;
        private readonly IHelperService _helperService;

        private readonly UserManager<User> _userManager;

        public MarkController(UserManager<User> userManager, IAreaService areaService, IMarkService markService, IClusterService clusterService, IKeyWordService keyWordService, IHelperService helperService, ICuratorService curatorService)
        {
            _userManager = userManager;
            _areaService = areaService;
            _markService = markService;
            _clusterService = clusterService;
            _keyWordService = keyWordService;
            _helperService = helperService;
            _curatorService = curatorService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id, bool isCluster = false)
        {
            if (isCluster)
            {
                var clusterDto = await _clusterService.GetClusterById(id);
                if (clusterDto is null)
                    return NotFound("Скупчення не знайдено");

                var revisionMarkViewModel = new RevisionMarkViewModel
                {
                    Id = clusterDto.Id,
                    Name = clusterDto.Name,
                    Description = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? clusterDto.Description : clusterDto.DescriptionEng,
                    CuratorName = clusterDto.Curator?.Name,
                    Area = clusterDto.Area is null ? "" : clusterDto.Area.Name + ", " + clusterDto.Area.Community + ", " + clusterDto.Area.Region,
                    Images = clusterDto.Marks.SelectMany(x => x.Images).Select(_helperService.GetImage).ToArray(),
                    ClusterId = null,
                    IsCluster = true
                };
                if (string.IsNullOrEmpty(clusterDto.ResourceUrl))
                {
                    if (clusterDto.Marks != null && clusterDto.Marks.Length > 0)
                    {
                        var markWithUrl = clusterDto.Marks.FirstOrDefault(x => !string.IsNullOrEmpty(x.ResourceUrl));
                        if (markWithUrl != null)
                        {
                            revisionMarkViewModel.ResourceUrl = markWithUrl.ResourceUrl;
                            revisionMarkViewModel.ResourceName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? markWithUrl.ResourceName: markWithUrl.ResourceNameEng;
                        }
                    }
                }
                else
                {
                    revisionMarkViewModel.ResourceUrl = clusterDto.ResourceUrl;
                    revisionMarkViewModel.ResourceName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? clusterDto.ResourceName : clusterDto.ResourceNameEng;
                }
                return View(revisionMarkViewModel);
            }

            var markDto = await _markService.GetMarkById(id);
            if (markDto is null || (!User.IsInRole("Admin") && !markDto.Images.Any(x => x.IsVisible)))
            {
                return RedirectToAction("NotFound", "Error", new { info = "Mark" });
            }
            else
            {
                //var descriptionEng = "";
                //descriptionEng = markDto.DescriptionEng is null or "" ? "This mark has no english description" : markDto.DescriptionEng;

                var revisionMarkViewModel = new RevisionMarkViewModel
                {
                    Id = markDto.Id,
                    Name = markDto.Name,
                    Description = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? markDto.Description : markDto.DescriptionEng,
                    ResourceUrl = markDto.ResourceUrl,
                    ResourceName = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ?  markDto.ResourceName: markDto.ResourceNameEng,
                    CuratorName = markDto.Curator.Name,
                    Area = markDto.Area is null ? "" : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                    ClusterId = markDto.Cluster?.Id,
                    ClusterName = markDto.Cluster?.Name,
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
                Name = markDto?.Name,
                Images = markDto is null ? Array.Empty<StringAndBoolViewModel>() : markDto.Images.Select(x => new StringAndBoolViewModel
                {

                    Content = _helperService.GetImage(x.Content),
                    IsVisible = x.IsVisible
                }).ToArray(),
                IsNew = markDto is null,
                Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                }).OrderBy(x => x.Name).ToArray(),
                Clusters = clusterDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray(),
                Curators = curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToArray(),
                //Places = markDto is null ? new GuidIdAndNameViewModel[0] : markDto.Places.Select( x => new GuidIdAndNameViewModel
                //{
                //    Id = x.Id,
                //    Name = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? x.Name : x.NameEng,
                //}).ToArray(),
                Places = (await _helperService.GetAllPlaces()).Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? x.Name : x.NameEng,
                }).ToArray(),
                SelectedPlaceIds = markDto is null ? Array.Empty<Guid>() : markDto.Places.Select(x => x.Id).ToArray()
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
                markSaveDto.PlaceIds = saveMarkViewModel.SelectedPlaceIds;

                if (saveMarkViewModel.ClusterId is not null)
                {
                    var clusterDto = await _clusterService.GetClusterById(saveMarkViewModel.ClusterId.Value);

                    if (clusterDto is null)
                        return Json(new SaveMarkResultViewModel
                        {
                            Id = null,
                            Success = false,
                            Url = "/Mark/Revision/{id}",
                            Text = "Вибраного скупчення не існує"
                        });
                    markSaveDto.Lng = clusterDto.Lng;
                    markSaveDto.Lat = clusterDto.Lat;
                }
                else
                {
                    if (saveMarkViewModel.Lng is null || saveMarkViewModel.Lat is null)
                        return Json(new SaveMarkResultViewModel
                        {
                            Id = null,
                            Success = false,
                            Url = "",
                            Text = "Виставіть координати мітці або виберіть скупчення для неї"
                        });
                    markSaveDto.Lng = double.Parse(saveMarkViewModel.Lng, CultureInfo.InvariantCulture);
                    markSaveDto.Lat = double.Parse(saveMarkViewModel.Lat, CultureInfo.InvariantCulture);
                }
                markSaveDto.Name = saveMarkViewModel.Name;
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
        public async Task<IActionResult> Catalogue()
        {
            var unitedCatalogueViewModel = new UnitedCatalogueViewModel();

            var markDtos = await _markService.GetMarks();
            var clusterDtos = await _clusterService.GetClusters();

            if (!User.IsInRole("Admin"))
            {
                markDtos = markDtos.Where(x => x.Images.Where(y => y.IsVisible).Any()).ToArray();
                clusterDtos = clusterDtos.Where(x => x.Marks.Length > 0 && x.Marks.Any(y => y.Images.Any())).ToArray();
            }

            unitedCatalogueViewModel.Names = (await _helperService.GetAllKeyWords()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).Concat(clusterDtos.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = x.Name
            })).ToArray();

            unitedCatalogueViewModel.Places = (await _helperService.GetAllPlaces()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? x.Name : x.NameEng,
            }).ToArray();

            unitedCatalogueViewModel.Areas = (await _areaService.GetUsedAreasAsync()).Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = x.Name + ", " + x.Community + ", " + x.Region,
            }).ToArray();


            unitedCatalogueViewModel.Marks = markDtos.Select(x => new MarkCatalogueViewModel
            {
                Id = x.Id,
                Name = x.Name,
                CuratorName = x.Curator.Name,
                //Description = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ua" ? x.Description : x.DescriptionEng,
                Area = x.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel { Id = x.Area.Id, Name = x.Area.Name },
                Images = x.Images?.Select(x => _helperService.GetImage(x.Content)).ToArray(),
            })
                .Concat(clusterDtos.Select(x => new MarkCatalogueViewModel
                {
                    Id = x.Id,
                    IsCluster = true,
                    Name = x.Name,
                    CuratorName = x.Curator?.Name,
                    Area = x.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel { Id = x.Area.Id, Name = x.Area.Name },
                    Images = x.Marks?.SelectMany(y => y.Images.Select(a => _helperService.GetImage(a))).ToArray()
                }))
                .ToArray();

            return View("Catalogue", unitedCatalogueViewModel);
        }

        public async Task<IActionResult> GetImages(Guid id, bool isCluster, int index)
        {
            var revisionImageViewModel = new RevisionImageViewModel();
            if (isCluster)
            {
                var clusterDto = await _clusterService.GetClusterById(id);
                revisionImageViewModel = new RevisionImageViewModel
                {
                    IsCluster = isCluster,
                    Id = clusterDto.Id,
                    Images = clusterDto.Marks.SelectMany(x => x.Images).Select(x => _helperService.GetImage(x)).ToArray()
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


        //[AllowAnonymous]
        //public async Task<IActionResult> Catalogue()
        //{
        //    var markCatalogueFilterViewModel = new MarkCatalogueFilter()
        //    {
        //        KeyWordAndMarkIds = (await _keyWordService.GetKeyWords())
        //            .Select(x => new StringIdAndStringName
        //            {
        //                Id = "K" + x.Id,
        //                Name = x.Name
        //            }).ToArray().Concat((await _markService.GetMarkNames()).Select(x => new StringIdAndStringName
        //            {
        //                Id = "M" + x.Id,
        //                Name = x.Name
        //            })).ToArray(),
        //        AreaIds = (await _areaService.GetUsedAreasAsync()).Select(x => new GuidIdAndStringName
        //        {
        //            Id = x.Id,
        //            Name = x.Name + ", " + x.Community + ", " + x.Region
        //        }).ToArray(),
        //        AreaName = "",
        //        MarkDescription = "",
        //        //MarkName = "",
        //        NotIncludeCluster = false
        //    };

        //    return View("Catalogue", markCatalogueFilterViewModel);
        //}

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetMarks([FromBody] MarkCatalogueFilter filter)
        {
            var markIds = filter.SelectedKeyWordAndMarkIds.Where(x => x.StartsWith("M")).Select(x => Guid.Parse(x[1..]));
            var keyWordIds = filter.SelectedKeyWordAndMarkIds.Where(x => x.StartsWith("K")).Select(x => Guid.Parse(x[1..]));

            var markDtos = await _markService.GetMarksByFilters(
                filter.AreaName,
                keyWordIds.ToArray(),
                markIds.ToArray(),
                filter.SelectedAreaIds,
                filter.MarkDescription,
                filter.NotIncludeCluster);

            if (!filter.FullData)
            {
                return Json(markDtos.Select(x => new MarkCatalogueViewModelShortData
                {
                    Area = x.Area is null ? null : new GuidIdAndStringName
                    {
                        Id = x.Area.Id,
                        Name = x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region,
                    },
                    Name = x.Name,
                    Id = x.Id,
                    Description = x.Description,
                    DescriptionEng = x.DescriptionEng
                }).ToArray());
            }
            else
            {
                var markCatalogueViewModels = markDtos.OrderByDescending(x => x.CreatedDate).Select(x => new MarkCatalogueViewModel
                {
                    Area = new GuidIdAndNameViewModel
                    {
                        Id = x.Area.Id,
                        Name = x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region,
                    },
                    Name = x.Name,
                    Id = x.Id,
                    Description = x.Description,
                    DescriptionEng = x.DescriptionEng,
                    ResourceUrl = x.ResourceUrl,
                    Images = x.Images.Select(y => _helperService.GetImage(y.Content)).ToArray()
                }).ToArray();

                if (filter.From.HasValue)
                {
                    markCatalogueViewModels = markCatalogueViewModels.Skip(filter.From.Value).ToArray();
                }

                if (filter.Count.HasValue && markCatalogueViewModels.Length >= filter.Count.Value)
                {
                    markCatalogueViewModels = markCatalogueViewModels.Take(filter.Count.Value).ToArray();
                }
                return Json(markCatalogueViewModels);
            }
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
        public async Task<IActionResult> Delete(Guid? id)
        {
            MarkDto? markDto = null;

            if (id is not null)
            {
                markDto = await _markService.GetMarkById(id.Value);
            }

            if (markDto is null)
                return RedirectToAction("NotFound", "Error", new { info = "Mark" });

            var areaDtos = await _areaService.GetAreasByNameAsync(markDto.Area is null ? "Херсон" : markDto.Area.Name);

            var addMarkViewModel = new AddMarkViewModel()
            {
                Lat = markDto.Lat?.ToString(CultureInfo.InvariantCulture),
                Lng = markDto.Lng?.ToString(CultureInfo.InvariantCulture),
                Zoom = "9",
                AreaId = markDto.Area?.Id,
                AreaName = markDto.Area is null ? null : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                Description = markDto.Description,
                DescriptionEng = markDto.DescriptionEng,
                ResourceUrl = markDto.ResourceUrl,
                Name = markDto.Name,
                Images = markDto.Images.Select(x => new StringAndBoolViewModel
                {
                    Content = _helperService.GetImage(x.Content),
                    IsVisible = x.IsVisible
                }).ToArray(),
                IsNew = false,
                Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                }).OrderBy(x => x.Name).ToArray(),
                ToDelete = true
            };

            return View("Add", addMarkViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteMark()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ImageToInsert([FromBody]string content)
        {
            return PartialView("_ImageToInsert", new StringAndBoolViewModel { 
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

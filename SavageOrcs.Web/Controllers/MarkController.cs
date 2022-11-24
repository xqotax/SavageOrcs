using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DbContext;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using SavageOrcs.DataTransferObjects.Marks;
using System.Text;
using NuGet.Versioning;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Maps;
using SavageOrcs.DataTransferObjects.Areas;

namespace SavageOrcs.Web.Controllers
{
    public class MarkController : Controller
    {
        private readonly ILogger<MarkController> _logger;
        private readonly IAreaService _areaService;
        private readonly IMarkService _markService;
        private UserManager<User> _userManager;

        public MarkController(ILogger<MarkController> logger, UserManager<User> userManager, IAreaService areaService, IMarkService markService)
        {
            _logger = logger;
            _userManager = userManager;
            _areaService = areaService;
            _markService = markService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id)
        {
            var markDto = await _markService.GetMarkById(id);
            if (markDto is null)
            {
                return NotFound();
            }
            else
            {
                var descriptionEng = "";
                if ((markDto.DescriptionEng is null) || (markDto.DescriptionEng == ""))
                    descriptionEng = "This mark has no english description";
                else
                {
                    descriptionEng = markDto.DescriptionEng;
                }

                var revisionMarkViewModel = new RevisionMarkViewModel
                {
                    Id = markDto.Id,
                    Lat = markDto.Lat,
                    Lng = markDto.Lng,
                    Name = markDto.Name,
                    Description = markDto.Description,
                    DescriptionEng = descriptionEng,
                    ResourceUrl = markDto.ResourceUrl,
                    Area = markDto.Area is null ? "" : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                    Images = markDto.Images?.Select(x => GetImage(x)).ToArray()
                };

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

            if (markDto is null)
            {
                areaDtos = await _areaService.GetAreasByNameAsync("Херсон");
            }
            else
            {
                areaDtos = await _areaService.GetAreasByNameAsync(markDto.Area is null ? "Херсон" : markDto.Area.Name);
            }
                
            var addMarkViewModel = new AddMarkViewModel()
            {
                Lat = markDto is null ? "48.6125528" : markDto.Lat.ToString(CultureInfo.InvariantCulture),
                Lng = markDto is null ? "31.0275809" : markDto.Lng.ToString(CultureInfo.InvariantCulture),
                Zoom = markDto is null ? "6" : "9",
                AreaId = markDto?.Area?.Id,
                AreaName = markDto?.Area is null ? null : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                Description = markDto?.Description,
                DescriptionEng = markDto?.DescriptionEng,
                ResourceUrl = markDto?.ResourceUrl,
                Name = markDto?.Name,
                Images = markDto?.Images?.Select(x => GetImage(x)).ToArray(),
                IsNew = markDto is null,
                Areas = areaDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Name = x.Name + ", " + x.Community + ", " + x.Region,
                    Id = x.Id
                }).OrderBy(x => x.Name).ToArray()
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
                markSaveDto.UserId = _userManager.GetUserId(User);
                markSaveDto.AreaId = saveMarkViewModel.AreaId;
                markSaveDto.Lng = double.Parse(saveMarkViewModel.Lng, CultureInfo.InvariantCulture);
                markSaveDto.Lat = double.Parse(saveMarkViewModel.Lat, CultureInfo.InvariantCulture);
                markSaveDto.Name = saveMarkViewModel.Name;
                markSaveDto.Description = saveMarkViewModel.Description;
                markSaveDto.DescriptionEng = saveMarkViewModel.DescriptionEng;
                markSaveDto.ResourceUrl = saveMarkViewModel.ResourceUrl;
                markSaveDto.MapId = 1;
                markSaveDto.Images = saveMarkViewModel.Images.Select(x => GetBytes(x)).ToArray();
            }
            catch
            {
                //error
            }

            var markSaveResultDto = await _markService.SaveMark(markSaveDto);



            return Json(new SaveMarkResultViewModel
            {
                Id = markSaveResultDto.Id,
                Success = markSaveResultDto.Success,
                Url = "/Mark/Revision/{id}",
                Text = "Мітка успішно збережена"
            });
        }

        private static byte[] GetBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        private static string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        [AllowAnonymous]
        public IActionResult Catalogue()
        {
            var markCatalogueFilterViewModel = new MarkCatalogueFilter()
            {
                Area = "",
                KeyWord = ""
            };

            return View("Catalogue", markCatalogueFilterViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetMarks([FromBody] MarkCatalogueFilter filter)
        {
            var markDtos = await _markService.GetMarksByNameAndArea(filter.Area, filter.KeyWord);

            if (!filter.FullData)
            {
                return Json(markDtos.Select(x => new MarkCatalogueViewModelShortData
                { 
                    Area = x.Area is null ? new GuidIdAndStringName { Id = null, Name = ""} : new GuidIdAndStringName
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
                    Area = x.Area is null ? new GuidIdAndStringName { Id = null, Name = "" } : new GuidIdAndStringName
                    {
                        Id = x.Area.Id,
                        Name = x.Area.Name + ", " + x.Area.Community + ", " + x.Area.Region,
                    },
                    Name = x.Name,
                    Id = x.Id,
                    Description = x.Description,
                    DescriptionEng = x.DescriptionEng,
                    ResourceUrl = x.ResourceUrl,
                    Images = x.Images.Select(y => GetImage(y)).ToArray()
                }).ToArray();

                if (filter.From.HasValue)
                {
                    markCatalogueViewModels = markCatalogueViewModels.Skip(filter.From.Value).ToArray();
                }

                if (filter.Count.HasValue)
                {
                    switch (filter.From.HasValue)
                    {
                        case true:
                            if (markCatalogueViewModels.Length >= filter.From.Value + filter.Count.Value)
                            {
                                markCatalogueViewModels = markCatalogueViewModels.Take(filter.Count.Value).ToArray();
                            }
                            break;
                        case false:
                            if (markCatalogueViewModels.Length >= filter.Count.Value)
                            {
                                markCatalogueViewModels = markCatalogueViewModels.Take(filter.Count.Value).ToArray();
                            }
                            break;
                    }
                }
                return Json(markCatalogueViewModels);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
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
                return NotFound();

            var areaDtos = await _areaService.GetAreasByNameAsync(markDto.Area is null ? "Херсон" : markDto.Area.Name);

            var addMarkViewModel = new AddMarkViewModel()
            {
                Lat = markDto.Lat.ToString(CultureInfo.InvariantCulture),
                Lng = markDto.Lng.ToString(CultureInfo.InvariantCulture),
                Zoom = "9",
                AreaId = markDto.Area?.Id,
                AreaName = markDto.Area is null ? null : markDto.Area.Name + ", " + markDto.Area.Community + ", " + markDto.Area.Region,
                Description = markDto.Description,
                DescriptionEng = markDto.DescriptionEng,
                ResourceUrl = markDto.ResourceUrl,
                Name = markDto.Name,
                Images = markDto.Images?.Select(x => GetImage(x)).ToArray(),
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
    }
}

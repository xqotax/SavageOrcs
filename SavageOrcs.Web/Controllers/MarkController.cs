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
                //error
            }

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
                Images = markDto.Images.Select(x => GetImage(x)).ToArray()
            };

            return View(revisionMarkViewModel);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var areaDtos = await _areaService.GetAreasAsync();

            var addMarkViewModel = new AddMarkViewModel()
            {
                Lat = "48.6125528",
                Lng = "31.0275809",
                Zoom = "6",
                AreaId = null,
            };

            return View(addMarkViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetAreas([FromBody] SearchAreasViewModel searcAreasViewModel)
        {
            var areaDtos = await _areaService.GetAreasAsync();
            var areaDropDownList = areaDtos.Where(x => x.Name.StartsWith(searcAreasViewModel.Text.ToUpper()))
                .Select(x => new GuidIdAndNameViewModel
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

        private byte[] GetBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        //Base64StringToByteArray
        //var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
        //return Convert.FromBase64String(base64Data);
        }
        private string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
            //ByteArrayToBase64String
            //return Convert.ToBase64String(data);
        }

        [AllowAnonymous]
        public IActionResult Catalogue()
        {
            var markCatalogueFilterViewModel = new MarkCatalogueFilters()
            {
                Area = "",
                KeyWord = ""
            };

            return View("Catalogue", markCatalogueFilterViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetMarks([FromBody] MarkCatalogueFilters filter)
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
                return Json(markDtos.Select(x => new MarkCatalogueViewModel
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
                }).ToArray());
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Area;
using SavageOrcs.Web.ViewModels.Constants;
using System.Data;

namespace SavageOrcs.Web.Controllers
{
    public class AdminHelperController : Controller
    {
        private readonly IHelperService _helperService;
        private readonly IAreaService _areaService;

        public AdminHelperController(IHelperService helperService, IAreaService areaService)
        {
            _helperService = helperService;
            _areaService = areaService;
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> KeyWordCatalogue()
        {
            var keyWords = await _helperService.GetAllKeyWords();

            return View(keyWords.Select(x => new GuidIdAndNameViewModelWithEnglishName
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng
            }).ToArray());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveKeyWords([FromBody] StringIdAndNameViewModelWhitEngName[] dataArray)
        {
            var keyWordDtos = dataArray.Select(x => new GuidNullIdAndStringNameWhitEngName { 
                Id = x.Id == "" || x.Id is null ? null : Guid.Parse(x.Id), 
                Name = x.Name,
                NameEng = x.NameEng
            }).ToArray();

            await _helperService.SaveKeyWords(keyWordDtos);

            return Json(null);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AreaCatalogue()
        {
            var areas = await _areaService.GetUsedAreasAsync();
            var areaViewModels = areas.Select(x => new AreaCatalogueViewModel
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                NameEng = x.NameEng,
                Community = x.Community,
                CommunityEng = x.CommunityEng,
                Region = x.Region,
                RegionEng = x.RegionEng
            }).ToArray();

            return View(areaViewModels);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveAreas([FromBody] AreaCatalogueViewModel[] dataArray)
        {
            var areaDtos = dataArray.Select(x => new AreaSaveDto {
                Id = string.IsNullOrEmpty(x.Id) ? null : Guid.Parse(x.Id), 
                Name = x.Name ,
                NameEng = x.NameEng,
                Community = x.Community,
                CommunityEng = x.CommunityEng,
                Region = x.Region,
                RegionEng = x.RegionEng
            }).ToArray();

            foreach (var area in areaDtos)
            {
                await _areaService.SaveArea(area);
            }

            return Json(null);
        }

        
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using System.Data;

namespace SavageOrcs.Web.Controllers
{
    public class AdminHelperController : Controller
    {
        private readonly IHelperService _helperService;

        public AdminHelperController(IHelperService helperService)
        {
            _helperService = helperService;
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> KeyWordCatalogue()
        {
            var keyWords = await _helperService.GetAllKeyWords();

            return View(keyWords.Select(x => new GuidIdAndNameViewModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToArray());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveKeyWords([FromBody] StringIdAndNameViewModel[] dataArray)
        {
            var keyWordDtos = dataArray.Select(x => new GuidNullIdAndStringName { Id = x.Id == "" || x.Id is null ? null : Guid.Parse(x.Id), Name = x.Name }).ToArray();

            await _helperService.SaveKeyWords(keyWordDtos);

            return Json(null);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PlaceCatalogue()
        {
            var places = await _helperService.GetAllPlaces();

            return View(places.Select(x => new GuidIdAndNameViewModelWithEnglishName
            {
                Id = x.Id,
                Name = x.Name,
                NameEng = x.NameEng
            }).ToArray());
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SavePlaces([FromBody] StringIdAndNameViewModelWhitEngName[] dataArray)
        {
            var placeDtos = dataArray.Select(x => new GuidNullIdAndStringNameWhitEngName {
                Id = x.Id == "" || x.Id is null ? null : Guid.Parse(x.Id), 
                Name = x.Name ,
                NameEng = x.NameEng
            }).ToArray();

            await _helperService.SavePlaces(placeDtos);

            return Json(null);
        }

        
    }
}

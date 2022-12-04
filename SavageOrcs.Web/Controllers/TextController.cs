using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Texts;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.Text;
using System.Globalization;

namespace SavageOrcs.Web.Controllers
{
    public class TextController : Controller
    {
        private readonly ILogger<TextController> _logger;
        private readonly ITextService _textService;
        private readonly ICuratorService _curatorService;

        public TextController(ILogger<TextController> logger, ITextService textService, ICuratorService curatorService)
        {
            _logger = logger;
            _textService = textService;
            _curatorService = curatorService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id) { 

            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {
            TextDto? textDto = null;

            if (id is not null)
                textDto = await _textService.GetTextById(id.Value);

            var curatorDtos = await _curatorService.GetCurators();

            var addTextViewModel = new AddTextViewModel()
            {
                Id = textDto?.Id,
                Name = textDto?.Name,
                Subject = textDto?.Subject,
                CuratorId = textDto?.Curator?.Id,
                CuratorName = textDto?.Curator?.Name,
                Curators = curatorDtos.Select(x => new GuidIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.DisplayName
                }).ToArray(),
                ToDelete = false,
                IsNew = textDto is null
            }; 

            return View(addTextViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveText([FromBody] AddTextViewModel saveTextViewModel)
        {
            var textSaveDto = new TextSaveDto
            {
                Id = saveTextViewModel?.Id,
                Subject = saveTextViewModel?.Subject,
                Name = saveTextViewModel?.Name, 
                CuratorId = saveTextViewModel?.CuratorId,
                Content = saveTextViewModel?.Content
            };
            var result = await _textService.SaveText(textSaveDto);

            return Json(new SaveMarkResultViewModel
            {
                Id = result.Id,
                Success = result.Success,
                Url = "/Text/Revision/{id}",
                Text = "Текст успішно збережений"
            });
        }


        [AllowAnonymous]
        public async Task<IActionResult> Catalogue()
        {
            var curatorDtos = await _curatorService.GetCurators();

            var filterCatalogueTextViewModel = new FilterCatalogueTextViewModel
            {
                CuratorName = "",
                TextName = "",
                TextSubject = "",
                Curators = curatorDtos.Select(x => x.DisplayName).ToArray()
            };

            return View(filterCatalogueTextViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> GetTexts([FromBody] FilterCatalogueTextViewModel filters)
        {
            var textDtos = await _textService.GetTextsByFilters(filters.TextName, filters.TextSubject, filters.CuratorName);

            var textCatalogueViewModel = textDtos.Select(x => new TextCatalogueViewModel { 
                Id = x.Id,
                Subject = x.Subject,
                Name = x.Name,
                Curator = x.Curator is null ? null: new GuidIdAndNameViewModel { Id = x.Curator.Id, Name = x.Curator.Name} 
            }).ToArray();

            return Json(textCatalogueViewModel);
        }


    }
}

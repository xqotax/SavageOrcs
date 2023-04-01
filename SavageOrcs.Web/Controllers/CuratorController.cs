using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.DataTransferObjects.Curators;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Cluster;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Curator;
using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.Text;
using System.Globalization;
using System.Text;

namespace SavageOrcs.Web.Controllers
{
    public class CuratorController : Controller
    {
        private readonly IHelperService _imageService;
        private readonly ICuratorService _curatorService;
        private readonly ITextService _textService;
        private readonly IMarkService _markService;
        private readonly IHelperService _helperService;

        public CuratorController(ICuratorService curatorService, IHelperService imageService, ITextService textService, IMarkService markService, IHelperService helperService)
        {
            _curatorService = curatorService;
            _imageService = imageService;
            _textService = textService;
            _markService = markService;
            _helperService = helperService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(Guid? id)
        {

            var curatorDto = id.HasValue ? await _curatorService.GetCuratorById(id.Value) : null;

            var addCuratorViewModel = new AddCuratorViewModel();

            addCuratorViewModel.Id = curatorDto?.Id;
            addCuratorViewModel.DisplayName = curatorDto?.DisplayName;
            addCuratorViewModel.Description = curatorDto?.Description;
            addCuratorViewModel.DescriptionEng = curatorDto?.DescriptionEng;
            addCuratorViewModel.Image = curatorDto is null || curatorDto.Image is null ? null : _helperService.GetImage(curatorDto.Image);

            return View(addCuratorViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> SaveCurator([FromBody] AddCuratorViewModel saveCuratorViewModel)
        {
            if (string.IsNullOrEmpty(saveCuratorViewModel.Image))
                return Json(new ResultViewModel
                {
                    Id = saveCuratorViewModel.Id,
                    Success = false,
                    Url = "/Curator/Add",
                    Text = "Вставте будь-ласка фото автора!"
                });

            var curatorSaveDto = new CuratorSaveDto
            {
                Id = saveCuratorViewModel.Id,
                DisplayName = saveCuratorViewModel.DisplayName,
                Description = saveCuratorViewModel.Description,
                DescriptionEng = saveCuratorViewModel.DescriptionEng,
                Image = _helperService.GetBytes(saveCuratorViewModel.Image)
            };

            var result = await _curatorService.SaveCurator(curatorSaveDto);

            return Json(new ResultViewModel
            {
                Id = result.Id,
                Success = result.Success,
                Url = "/Curator/Catalogue",
                Text = result.Success ? "Автор успішно збережений": "Something Went Wrong"
            });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddImage()
        {
            return PartialView("_AddImage");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCurator()
        {
            return PartialView("_Delete");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> DeleteConfirm(Guid id)
        {
            var result = await _curatorService.DeleteCurator(id);


            return Json(new ResultViewModel
            {
                Id = id,
                Success = result,
                Url = "/Curator/Catalogue",
                Text = result ? "Куратор успішно видалений" : "Помилка, зверніться до адміністратора"
            });
        }


        [AllowAnonymous]
        public async Task<IActionResult> Catalogue()
        {
            var curatorDtos = await _curatorService.GetCurators();

            var curators = !curatorDtos.Any() ? Array.Empty<CuratorViewModel>() : curatorDtos.Select(x => new CuratorViewModel
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Description = _helperService.GetTranslation(x.Description, x.DescriptionEng),
                Image = x.Image is not null ? _imageService.GetImage(x.Image) : null
            }).ToArray();
            

            foreach (var curatorViewModel in curators)
            {
                curatorViewModel.Texts = (await _textService.GetTextsByCuratorIds(curatorViewModel.Id.Value))
                    .Select(y => new TextCatalogueViewModel
                    {
                        Id = y.Id,
                        CreatedDate = _helperService.GetTranslation(y.CreatedDate.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("uk-UA")),y.CreatedDate.ToString("dd/MM/yyyy")),
                        Name = y.Name
                    }).ToArray();
                curatorViewModel.TextCount = curatorViewModel.Texts.Length;
                curatorViewModel.Marks = (await _markService.GetMarksByCuratorIds(curatorViewModel.Id.Value))
                    .Select(y => new MarkCatalogueViewModel
                    {
                        Id = y.Id,
                        Name = y.Name,
                        ResourceName = _helperService.GetTranslation(y.ResourceName, y.ResourceNameEng),
                        Area = y.Area is null ? new GuidIdAndNameViewModel() : new GuidIdAndNameViewModel
                        {
                            Id = y.Area.Id,
                            Name = y.Area.Name,
                        }
                    }).OrderByDescending(x => x.Name).ToArray();
                curatorViewModel.MarkCount = curatorViewModel.Marks.Length;


            }

            return View("Catalogue", curators);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id)
        {
            var curatorDto = await _curatorService.GetCuratorById(id);

            var curatorRevisionViewModel = new RevisionCuratorViewModel
            {
                Id = curatorDto.Id,
                DisplayName = curatorDto.DisplayName,
                Description = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "uk" ? curatorDto.Description: curatorDto.DescriptionEng,
                Image = curatorDto.Image is not null ? _imageService.GetImage(curatorDto.Image) : null,
                Texts = curatorDto.TextDtos?.Select(x => new RevisionCuratorTextsViewModel {
                    Id = x.Id,
                    Name = x.Name,
                    Subject = x.Subject
                }).ToArray()
            };
            return View(curatorRevisionViewModel);
        }


        //[Authorize(Roles = "Global admin")]
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var curatorDto = await _curatorService.GetCuratorById(id);

        //    return RedirectToAction("Revision", "User", new { id = curatorDto.UserId });
        //}
    }
}

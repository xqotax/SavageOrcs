using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Curator;
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

        public CuratorController(ICuratorService curatorService, IHelperService imageService, ITextService textService)
        {
            _curatorService = curatorService;
            _imageService = imageService;
            _textService = textService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Catalogue()
        {
            var curatorDtos = await _curatorService.GetCurators();

            var curatorViewModels = new CuratorViewModels
            {
                Curators = !curatorDtos.Any() ? Array.Empty<CuratorViewModel>() : curatorDtos.Select(x => new CuratorViewModel
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Description = x.Description,
                    Image = x.Image is not null ? _imageService.GetImage(x.Image) : null
                }).ToArray()
            };

            foreach (var curatorViewModel in curatorViewModels.Curators)
            {
                curatorViewModel.Texts = (await _textService.GetTextsByCuratorIds(curatorViewModel.Id.Value))
                    .Select(y => new TextCatalogueViewModel
                    {
                        Id = y.Id,
                        CreatedDate = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ua" 
                        ? y.CreatedDate.ToString("dd MMMM yyyy", CultureInfo.CreateSpecificCulture("uk-UA"))
                        : y.CreatedDate.ToString("dd/MM/yyyy"),

                        Name = y.Name
                    }).ToArray();
                curatorViewModel.TextCount = curatorViewModel.Texts.Length;
            }

            return View("Catalogue", curatorViewModels);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Revision(Guid id)
        {
            var curatorDto = await _curatorService.GetCuratorById(id);

            var curatorRevisionViewModel = new RevisionCuratorViewModel
            {
                Id = curatorDto.Id,
                DisplayName = curatorDto.DisplayName,
                Description = curatorDto.Description,
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

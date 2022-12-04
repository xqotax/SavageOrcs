using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Curator;
using System.Text;

namespace SavageOrcs.Web.Controllers
{
    public class CuratorController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ICuratorService _curatorService;

        public CuratorController(ICuratorService curatorService, IImageService imageService)
        {
            _curatorService = curatorService;
            _imageService = imageService;
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
                    UserId = x.UserId,
                    Image = x.Image is not null ? _imageService.GetImage(x.Image) : null
                }).ToArray()
            };
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


        [Authorize(Roles = "Global admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var curatorDto = await _curatorService.GetCuratorById(id);

            return RedirectToAction("Revision", "User", new { id = curatorDto.UserId });
        }
    }
}

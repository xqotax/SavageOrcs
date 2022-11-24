using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Curator;
using System.Text;

namespace SavageOrcs.Web.Controllers
{
    public class CuratorController : Controller
    {
        private readonly ICuratorService _curatorService;

        public CuratorController(ICuratorService curatorService)
        {
            _curatorService = curatorService;
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
                    Image = x.Image is not null ? GetImage(x.Image) : null
                }).ToArray()
            };
            return View("Catalogue", curatorViewModels);
        }
        
        private static string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
    }
}

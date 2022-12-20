using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Users;
using SavageOrcs.Services;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Curator;
using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.User;
using System.Data;
using System.Globalization;
using System.Text;

namespace SavageOrcs.Web.Controllers
{
    public class UserController : Controller
    {

        private readonly IRoleService _roleService;
        private readonly IUserService _userService;


        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Catalogue()
        {
            return View();
        }

        [Authorize(Roles = "Global admin")]
        public async Task<IActionResult> Revision(string id)
        {
            var userDto = await _userService.GetUserById(id);
            var userRoleIds = await _roleService.GetUserRoles(id);
            var allRoleDtos = await _roleService.GetAllRoles();

            var userViewModel = new UserRevisionViewModel
            {
                Id = userDto.Id,
                RoleIds = userRoleIds,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                IsCurator = userDto.CuratorDto is not null,
                CuratorInfo = new CuratorViewModel
                {
                    Id = userDto.CuratorDto is not null ? userDto.CuratorDto.Id: null,
                    DisplayName = userDto.CuratorDto is not null ? userDto.CuratorDto.DisplayName : null,
                    Description = userDto.CuratorDto is not null ? userDto.CuratorDto.Description : null,
                    Image = userDto.CuratorDto?.Image is not null ? GetImage(userDto.CuratorDto.Image) : null,
                }, 
                AllRoles = allRoleDtos.Select(x => new StringIdAndNameViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray()
            };

            return View(userViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetUsers([FromBody] UserCatalogueFilter filter)
        {
            var userDtos = await _userService.GetUsers(filter.Name, filter.Email);

            var userViewModels = userDtos.Select(x => new UserCatalogueViewModel
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                IsCurator = x.CuratorDto is not null
            }).ToArray();


            return Json(userViewModels);
        }

        private static byte[] GetBytes(string data)
        {
            return Encoding.ASCII.GetBytes(data);
        }
        private static string GetImage(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }
        [HttpPost]
        [Authorize(Roles = "Global admin")]
        public async Task<JsonResult> SaveUser([FromBody] SaveUserViewModel saveUserViewModel)
        {
            var userSaveDto = new UserSaveDto
            {
                Id = saveUserViewModel.Id,
                DisplayName = saveUserViewModel.DisplayName,
                Email = saveUserViewModel.Email,
                FirstName = saveUserViewModel.FirstName,
                LastName = saveUserViewModel.LastName,
                CuratorId = saveUserViewModel.CuratorId,
                RoleIds = saveUserViewModel.RoleIds,
                Description = saveUserViewModel.Description,
                IsCurator = saveUserViewModel.IsCurator,
                CuratorImage = saveUserViewModel.Image is null ? null : GetBytes(saveUserViewModel.Image)
            };

            var result = await _userService.SaveUser(userSaveDto);


            return Json(new SaveUserResultViewModel
            {
                Id = saveUserViewModel.Id,
                Success = result.Success,
                Url = "/Curator/Catalogue",
                Text = "Статус користувача збережено"
            });
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddCuratorImage()
        {
            return PartialView("_AddCuratorImage");
        }
    }
}

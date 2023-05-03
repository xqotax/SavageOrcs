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
        private readonly IHelperService _helperService;


        public UserController(IUserService userService, IRoleService roleService, IHelperService helperService)
        {
            _userService = userService;
            _roleService = roleService;
            _helperService = helperService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Catalogue()
        {

            var userDtos = await _userService.GetUsers("", "");

            var userViewModels = userDtos.Select(x => new UserCatalogueViewModel
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
            }).ToArray();

            return View(userViewModels);
        }

        [Authorize(Roles = "Global admin")]
        [EmailConfirmed]
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
        public async Task<JsonResult> GetUsers([FromBody] UserCatalogueFilter userCatalogueFilter)
        {
            var userDtos = await _userService.GetUsers(userCatalogueFilter.Name, userCatalogueFilter.Email);

            var userViewModels = userDtos.Select(x => new UserCatalogueViewModel
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
            }).ToArray();


            return Json(userViewModels);
        }

        [HttpPost]
        [Authorize(Roles = "Global admin")]
        public async Task<JsonResult> SaveUser([FromBody] SaveUserViewModel saveUserViewModel)
        {
            var userSaveDto = new UserSaveDto
            {
                Id = saveUserViewModel.Id,
                Email = saveUserViewModel.Email,
                FirstName = saveUserViewModel.FirstName,
                LastName = saveUserViewModel.LastName,
                RoleIds = saveUserViewModel.RoleIds,
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
    }
}

using Microsoft.AspNetCore.Identity;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Roles;
using SavageOrcs.Repositories.Interfaces;
using SavageOrcs.Services.Interfaces;
using SavageOrcs.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services
{
    public class RoleService : UnitOfWorkService, IRoleService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : base(unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string[]> GetUserRoles (string userId)
        {
            var user = await _userManager.FindByIdAsync (userId);

            var roleNames = await _userManager.GetRolesAsync(user);

            var roleIds = new List<string>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roleIds.Add(role.Id);
            }

            return roleIds.ToArray();
        }

        public async Task<RoleDto[]> GetAllRoles()
        {
            var user = await _userManager.FindByIdAsync("00010001-0001-0001-0001-000100010001");

            var roles = await _userManager.GetRolesAsync(user);

            var roleDtos = new List<RoleDto>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = roleName,
                });
            }

            return roleDtos.ToArray();
        }

    }
}

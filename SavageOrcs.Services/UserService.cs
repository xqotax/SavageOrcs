using Microsoft.AspNetCore.Identity;
using SavageOrcs.BusinessObjects;
using SavageOrcs.DataTransferObjects.Areas;
using SavageOrcs.DataTransferObjects.Curators;
using SavageOrcs.DataTransferObjects.Marks;
using SavageOrcs.DataTransferObjects.Roles;
using SavageOrcs.DataTransferObjects.Users;
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
    public class UserService : UnitOfWorkService, IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Curator> _curatorRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUnitOfWork unitOfWork, IRepository<User> userRepository, IRepository<Curator> curatorRepository, UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : base(unitOfWork)
        {
            _userRepository = userRepository;
            _curatorRepository = curatorRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserDto> GetUserById(string id)
        {
            var user = await _userRepository.GetTAsync(x => x.Id == id);

            user ??= new User();

            return CreateUserDto(user);
        }
        public async Task<UserDto[]> GetUsers(string fullName, string email)
        {
            var users = await _userRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(x => x.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                users = users.Where(x => (x.FirstName + " " + x.LastName).Contains(fullName) || (x.LastName + " " + x.FirstName).Contains(fullName));
            }  
            
            return users.Select(x => CreateUserDto(x)).ToArray();
        }

        private static UserDto CreateUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
            };
        }
        

        public async Task<UserSaveResultDto> SaveUser (UserSaveDto userSaveDto)
        {
            var user = await _userRepository.GetTAsync(x => x.Id == userSaveDto.Id);

            if (user == null)
                return new UserSaveResultDto()
                {
                    Success = false,
                    UserNotFound = true,
                    ErrorDuringSave = false
                };
            try
            {

                var oldRoleNames = await _userManager.GetRolesAsync(user);

                userSaveDto.RoleIds = userSaveDto.RoleIds is not null ? userSaveDto.RoleIds : Array.Empty<string>();
                var newRoleNames = new List<string>();
                foreach (var roleId in userSaveDto.RoleIds)
                {
                    var newRoleName = await _roleManager.FindByIdAsync(roleId);
                    newRoleNames.Add(newRoleName.Name);
                }

                //var mainDeveloper = await _userManager.FindByIdAsync("00010001-0001-0001-0001-000100010001");
                //var allRoleNames = await _userManager.GetRolesAsync(mainDeveloper);

                //oldRoleNames.Remove("Global admin");
                //newRoleNames.Remove("Global admin");

                foreach (var oldRoleName in oldRoleNames)
                {
                    if (!newRoleNames.Contains(oldRoleName))
                    {
                        await _userManager.RemoveFromRoleAsync(user, oldRoleName);
                    }
                }
                foreach (var newRoleName in newRoleNames)
                {
                    if (!oldRoleNames.Contains(newRoleName))
                    {
                        await _userManager.AddToRoleAsync(user, newRoleName);
                    }
                }

                user.FirstName = userSaveDto.FirstName;
                user.LastName = userSaveDto.LastName;
                user.Email = userSaveDto.Email;

                await UnitOfWork.SaveChangesAsync();
            }
            catch
            {
                return new UserSaveResultDto()
                {
                    Success = false,
                    ErrorDuringSave = false,
                    UserNotFound = true
                };
            }

            return new UserSaveResultDto()
            {
                Success = true,
                ErrorDuringSave = false,
                UserNotFound = false
            };
        }
    }
}

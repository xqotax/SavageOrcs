using SavageOrcs.DataTransferObjects.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface  IUserService
    {
        Task<UserDto[]> GetUsers(string fullName, string email);

        Task<UserDto> GetUserById(string id);

        Task<UserSaveResultDto> SaveUser(UserSaveDto userSaveDto);
    }
}

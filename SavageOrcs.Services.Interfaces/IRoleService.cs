using SavageOrcs.DataTransferObjects.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.Services.Interfaces
{
    public interface IRoleService
    {
        Task<string[]> GetUserRoles(string userId);

        Task<RoleDto[]> GetAllRoles ();
    }
}

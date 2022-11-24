using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavageOrcs.DataTransferObjects.Users
{
    public class UserSaveDto
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public bool IsCurator { get; set; }

        public string[]? RoleIds { get; set; }

        public string? CuratorId { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public byte[]? CuratorImage { get; set; }
    }
}

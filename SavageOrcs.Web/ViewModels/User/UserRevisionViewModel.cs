using SavageOrcs.Web.ViewModels.Constants;
using SavageOrcs.Web.ViewModels.Curator;

namespace SavageOrcs.Web.ViewModels.User
{
    public class UserRevisionViewModel
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string[]? RoleIds {get; set; }

        public StringIdAndNameViewModel[]? AllRoles { get; set; }
    }
}

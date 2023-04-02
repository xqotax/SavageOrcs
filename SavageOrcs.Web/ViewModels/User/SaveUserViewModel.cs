using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.User
{
    public class SaveUserViewModel
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string[]? RoleIds { get; set; }
    }
}

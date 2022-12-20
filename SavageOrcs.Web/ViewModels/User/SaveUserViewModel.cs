using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.User
{
    public class SaveUserViewModel
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

        public string? Image { get; set; }

    }
}

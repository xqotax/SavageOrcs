using SavageOrcs.Web.ViewModels.Cluster;

namespace SavageOrcs.Web.ViewModels.Curator
{
    public class RevisionCuratorViewModel
    {
        public Guid? Id { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public RevisionCuratorTextsViewModel[]? Texts { get; set; }
    }
}

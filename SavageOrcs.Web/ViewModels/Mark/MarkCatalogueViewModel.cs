using SavageOrcs.DataTransferObjects._Constants;
using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class MarkCatalogueViewModel
    {
        public GuidIdAndNameViewModel Area { get; set; } = new GuidIdAndNameViewModel();

        public string? Name { get; set; }

        public Guid? Id { get; set; }

        public string? CuratorName { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public bool IsCluster { get; set; } = false;

        public string? ClusterName { get; set; }

    }
}

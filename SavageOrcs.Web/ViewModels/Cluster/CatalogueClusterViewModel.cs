using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class CatalogueClusterViewModel
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public GuidIdAndNameViewModel? Area { get; set; }

        public int? MarkCount { get; set; } 
    }
}

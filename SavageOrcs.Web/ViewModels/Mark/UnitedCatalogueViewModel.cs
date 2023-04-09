using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class UnitedCatalogueViewModel
    {
        public GuidIdAndNameViewModel[] Areas { get; set; } = Array.Empty<GuidIdAndNameViewModel>();
        
        public GuidIdAndNameViewModel[] KeyWords { get; set; } = Array.Empty<GuidIdAndNameViewModel>();

        public GuidIdAndNameViewModel[] ClusterNames { get; set; } = Array.Empty<GuidIdAndNameViewModel>();

        public GuidIdAndNameViewModel[] MarkNames { get; set; } = Array.Empty<GuidIdAndNameViewModel>();

        public Guid[]? SelectedAreaIds { get; set; }
        public Guid[]? SelectedMarkIds { get; set; }
        public Guid[]? SelectedClusterIds { get; set; }
        public Guid[]? SelectedKeyWordIds { get; set; }
        public MarkCatalogueViewModel[] Marks { get; set; } = Array.Empty<MarkCatalogueViewModel>();
    }
}

using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class UnitedCatalogueViewModel
    {
        public GuidIdAndNameViewModel[] Areas { get; set; } = Array.Empty<GuidIdAndNameViewModel>();
        
        public GuidIdAndNameViewModel[] Places { get; set; } = Array.Empty<GuidIdAndNameViewModel>();

        public GuidIdAndNameViewModel[] Names { get; set; } = Array.Empty<GuidIdAndNameViewModel>();

        public Guid[]? AreaIds { get; set; }
        public Guid[]? KeyWordIds { get; set; }
        public Guid[]? ObjectIds { get; set; }

        public MarkCatalogueViewModel[] Marks { get; set; } = Array.Empty<MarkCatalogueViewModel>();
    }
}

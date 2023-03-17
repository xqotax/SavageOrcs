using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class UnitedCatalogueTextViewModel
    {
        public GuidIdAndNameViewModel[] Curators { get; set; } = Array.Empty<GuidIdAndNameViewModel>();
        public GuidIdAndNameViewModel[] TextNames { get; set; } = Array.Empty<GuidIdAndNameViewModel>();
        public Guid[]? CuratorIds { get; set; }
        public Guid[]? TextIds { get; set; }
        public TextRevisionViewModel[] Texts { get; set; } = Array.Empty<TextRevisionViewModel>();
    }
}

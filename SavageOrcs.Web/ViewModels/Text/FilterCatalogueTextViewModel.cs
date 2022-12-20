using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class FilterCatalogueTextViewModel
    {
        //public string? KeyWord { get; set; }

        public Guid[]? CuratorIds { get; set; }

        public string? TextName { get; set; }

        public string? TextSubject { get; set; }

        public GuidIdAndNameViewModel[] Curators { get; set; }
    }
}

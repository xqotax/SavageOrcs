using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class TextCatalogueViewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public string? CreatedDate { get; set; }  

        public GuidIdAndNameViewModel? Curator { get; set; }
    }
}

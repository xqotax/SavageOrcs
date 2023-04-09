using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class AddTextViewModel
    {
        public Guid? Id { get; set; }

        public Guid? CuratorId { get; set; }

        public string? CuratorName { get; set; }

        public Guid? UkrTextId { get; set; }

        public string? UkrTextName { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public TextBlockViewModel? Blocks { get; set; }

        public bool EnglishVersion { get; set; }

        public bool? IsNew { get; set; }

        public bool? ToDelete { get; set; }

        public GuidIdAndNameViewModel[]? Curators { get; set; }

        public GuidIdAndNameViewModel[]? UkrTexts { get; set; }
    }
}

using SavageOrcs.Web.ViewModels.Mark;
using SavageOrcs.Web.ViewModels.Text;

namespace SavageOrcs.Web.ViewModels.Curator
{
    public class CuratorViewModel
    {
        public Guid? Id { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public int TextCount { get; set; }

        public int MarkCount { get; set; }

        public MarkCatalogueViewModel[] Marks { get; set; } = Array.Empty<MarkCatalogueViewModel>();

        public TextCatalogueViewModel[] Texts { get; set; } = Array.Empty<TextCatalogueViewModel>();
    }
}

using SavageOrcs.Web.ViewModels.Constants;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class AddTextViewModel
    {
        public Guid? Id { get; set; }

        public Guid? CuratorId { get; set; }

        public string? CuratorName { get; set; }

        public string? Name { get; set; }

        public string? Subject { get; set; }

        public string? Content { get; set; }

        public bool? IsNew { get; set; }

        public bool? ToDelete { get; set; }

        public GuidIdAndNameViewModel[]? Curators { get; set; }
    }
}

using SavageOrcs.DataTransferObjects._Constants;

namespace SavageOrcs.Web.ViewModels.Cluster
{
    public class RevisionClusterMarksViewModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? DescriptionEng { get; set; }

        public string? ResourceUrl { get; set; }

        public string[]? Images { get; set; }

        public GuidIdAndStringName? Area { get; set; }
    }
}

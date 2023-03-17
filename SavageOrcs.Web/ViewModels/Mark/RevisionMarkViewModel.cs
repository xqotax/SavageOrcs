using SavageOrcs.DataTransferObjects.Areas;

namespace SavageOrcs.Web.ViewModels.Mark
{
    public class RevisionMarkViewModel
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ResourceUrl { get; set; }

        public string? ResourceName { get; set; }

        public Guid? ClusterId { get; set; }

        public string? ClusterName { get; set; } 

        public string? Area { get; set; }

        public string[]? Images { get; set; }

        public string? CuratorName { get; set; }

        public bool IsCluster { get; set; }
    }
}
